// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.UI;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.AssetsPreloader.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Services.AssetsPreloader
{
  /// <summary>
  /// Loads Addressable assets into their respective caches before the curtain hides,
  /// so that all gameplay-critical content is instantly available at runtime.
  ///
  /// Registration (DI):
  ///   builder.Bind&lt;IAssetsPreloader&gt;().To&lt;AssetsPreloader&gt;().AsSingle();
  /// </summary>
  public class AssetsPreloader : IAssetsPreloader
  {
    // ── Gameplay windows that must be warm in LoadLevelState ──────────────────
    // MainMenu / Shop / Credits are intentionally excluded: they are heavy and
    // not needed while the player is inside a level.
    private static readonly WindowTypeId[] GameplayWindows =
    {
      WindowTypeId.Inventory,
      WindowTypeId.Settings,
      WindowTypeId.ConfirmScreen,
    };

    private readonly ITrackLoader _trackLoader;
    private readonly IStaticDataService _staticData;
    private readonly IAssetLoader _assetLoader;

    public AssetsPreloader(
      ITrackLoader trackLoader,
      IStaticDataService staticData,
      IAssetLoader assetLoader)
    {
      _trackLoader = trackLoader;
      _staticData = staticData;
      _assetLoader = assetLoader;
    }

    #region IAssetsPreloader

    /// <inheritdoc/>
    public async UniTask PreloadMusicAsync(string sceneName, CancellationToken ct)
    {
      MusicPlaylist playlist = await _staticData.LevelMusic.ForLevelAsync(sceneName);

      if (ct.IsCancellationRequested || playlist == null)
        return;

      await _trackLoader.PreloadAllAsync(playlist, ct);
    }

    /// <inheritdoc/>
    public async UniTask PreloadSoundsAsync(CancellationToken ct)
    {
      // Warm up the prefabs that carry SoundComponent / SoundPlayer.
      // IAssetLoader caches the result, so the subsequent Instantiate calls
      // in GameFactory (CreateAndPlacePlayerAsync, CreateHudAsync) are instant.
      await UniTask.WhenAll(
        _assetLoader.LoadAsync<GameObject>(AssetAddresses.PlayerAddress),
        _assetLoader.LoadAsync<GameObject>(AssetAddresses.HudAddress)
      );
    }

    /// <inheritdoc/>
    public async UniTask PreloadUIAsync(CancellationToken ct)
    {
      // Load WindowStaticData + the prefab reference for every gameplay window
      // so IUIFactory.CreateWindow never stalls the first time it is called.
      var tasks = new UniTask[GameplayWindows.Length];

      for (int i = 0; i < GameplayWindows.Length; i++)
      {
        WindowTypeId windowId = GameplayWindows[i];
        tasks[i] = PreloadWindowAsync(windowId, ct);
      }

      await UniTask.WhenAll(tasks);
    }

    /// <inheritdoc/>
    public UniTask PreloadAllAsync(string sceneName, CancellationToken ct) =>
      UniTask.WhenAll(
        PreloadMusicAsync(sceneName, ct),
        PreloadSoundsAsync(ct),
        PreloadUIAsync(ct)
      );

    #endregion

    #region Private

    private async UniTask PreloadWindowAsync(WindowTypeId windowId, CancellationToken ct)
    {
      WindowStaticData data = await _staticData.WindowData.ForWindowAsync(windowId);

      if (ct.IsCancellationRequested || data == null)
        return;

      // Pre-cache the prefab so UIFactory.CreateWindow is a synchronous cache hit.
      await _assetLoader.LoadAsync<GameObject>(data.WindowReference);
    }

    #endregion
  }
}
