// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.CustomTypes.Types;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class LevelDataSubservice : ILevelDataSubservice
  {
    private LevelsManifestStaticData _manifest;

    private DictionaryData<string, LevelStaticData> _loadedLevels = new();

    private IAssetLoader _assetLoader;

    public LevelDataSubservice() => _assetLoader = RootContext.Resolve<IAssetLoader>();

    public async Task LoadSelfAsync() =>
      _manifest =
        await _assetLoader
        .LoadAsync<LevelsManifestStaticData>(StaticDataAddresses.LevelsManifestAddress);

    public async Task<LevelStaticData> ForLevelAsync(string sceneKey)
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
