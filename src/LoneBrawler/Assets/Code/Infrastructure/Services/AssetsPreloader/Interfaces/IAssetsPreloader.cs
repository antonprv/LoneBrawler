// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.AssetsPreloader.Interfaces
{
  /// <summary>
  /// Orchestrates upfront loading of assets so that gameplay states
  /// never stall on an addressable load at runtime.
  ///
  /// Call order for each state:
  ///   MainMenuState  → <see cref="PreloadMusicAsync"/>
  ///   LoadLevelState → <see cref="PreloadAllAsync"/> (music + sounds + UI)
  /// </summary>
  public interface IAssetsPreloader
  {
    /// <summary>
    /// Loads every AudioClip in the playlist that belongs to <paramref name="sceneName"/>
    /// into the track-loader cache.
    /// The curtain must stay visible until this completes.
    /// </summary>
    UniTask PreloadMusicAsync(string sceneName, CancellationToken ct);

    /// <summary>
    /// Warms up the Addressable prefabs that carry <c>SoundComponent</c>
    /// and <c>SoundPlayer</c> (Player, HUD), so the first instantiation
    /// is synchronous and never causes an audio hitch.
    /// </summary>
    UniTask PreloadSoundsAsync(CancellationToken ct);

    /// <summary>
    /// Loads the static data and prefab for every gameplay window
    /// (Inventory, Settings, ConfirmScreen, …) into the asset-loader cache,
    /// so <c>IUIFactory.CreateWindow</c> is instant when the player opens a window.
    /// </summary>
    UniTask PreloadUIAsync(CancellationToken ct);

    /// <summary>
    /// Convenience wrapper: runs <see cref="PreloadMusicAsync"/>,
    /// <see cref="PreloadSoundsAsync"/> and <see cref="PreloadUIAsync"/> in parallel.
    /// </summary>
    UniTask PreloadAllAsync(string sceneName, CancellationToken ct);
  }
}
