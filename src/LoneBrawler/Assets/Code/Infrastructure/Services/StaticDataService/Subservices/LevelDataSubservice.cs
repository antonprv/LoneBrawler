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
  public class LevelDataSubservice : ILevelDataSubservice
  {
    private LevelsManifestStaticData _manifest;

    private readonly DictionaryData<string, LevelStaticData> _loadedLevels = new();

    private readonly IAssetLoader _assetLoader;

    public LevelDataSubservice(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    public async UniTask LoadSelfAsync() =>
      _manifest =
        await _assetLoader
        .LoadAsync<LevelsManifestStaticData>(StaticDataAddresses.LevelsManifestAddress);

    public async UniTask<LevelStaticData> ForLevelAsync(string sceneKey)
    {
      if (_loadedLevels.TryGetValue(sceneKey, out LevelStaticData cached))
        return cached;

      _manifest.Levels.TryGetValue(sceneKey, out AssetReferenceT<LevelStaticData> entry);
      if (entry == null) return null;

      LevelStaticData data = await _assetLoader.LoadAsync<LevelStaticData>(entry);

      _loadedLevels[sceneKey] = data;
      return data;
    }
  }
}
