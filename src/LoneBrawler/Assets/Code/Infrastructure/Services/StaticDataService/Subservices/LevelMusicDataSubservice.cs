// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class LevelMusicDataSubservice : ILevelMusicDataSubservice
  {
    private readonly IAssetLoader _assetLoader;

    private LevelMusicManifestStaticData _manifest;

    private readonly DictionaryData<string, MusicPlaylist> _loadedPlaylists = new();

    public LevelMusicDataSubservice(IAssetLoader assetLoader) =>
      _assetLoader = assetLoader;

    public async UniTask LoadSelfAsync() =>
      _manifest =
        await _assetLoader
        .LoadAsync<LevelMusicManifestStaticData>(StaticDataAddresses.LevelMusicManifestAddress);

    public async UniTask<MusicPlaylist> ForLevelAsync(string sceneKey)
    {
      if (_loadedPlaylists.TryGetValue(sceneKey, out MusicPlaylist cached))
        return cached;

      _manifest.PlaylistsByLevel.TryGetValue(sceneKey, out AssetReferenceT<MusicPlaylist> entry);
      if (entry == null) return null;

      MusicPlaylist playlist = await _assetLoader.LoadAsync<MusicPlaylist>(entry);

      _loadedPlaylists[sceneKey] = playlist;
      return playlist;
    }
  }
}
