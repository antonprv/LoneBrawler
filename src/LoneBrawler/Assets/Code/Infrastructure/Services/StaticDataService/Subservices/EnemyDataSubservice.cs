// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.CustomTypes.Types;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class EnemyDataSubservice : IEnemyDataSubservice
  {
    private EnemyManifestStaticData _manifest;

    private DictionaryData<EnemyTypeId, EnemyStaticData> _loadedEnemies = new();

    private IAssetLoader _assetLoader;

    public EnemyDataSubservice() => _assetLoader = RootContext.Resolve<IAssetLoader>();

    public async Task LoadSelfAsync() =>
      _manifest = await _assetLoader
      .LoadAsync<EnemyManifestStaticData>(StaticDataAddresses.EnemyManifestAddress);

    public async Task<EnemyStaticData> ForEnemyAsync(EnemyTypeId typeId)
    {
      if (_loadedEnemies.TryGetValue(typeId, out EnemyStaticData cached))
        return cached;

      _manifest.Enemies.TryGetValue(typeId, out AssetReferenceT<EnemyStaticData> entry);
      if (entry == null) return null;

      EnemyStaticData data = await _assetLoader.LoadAsync<EnemyStaticData>(entry);

      _loadedEnemies[typeId] = data;
      return data;
    }
  }
}
