// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Enemies;

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
  public class EnemyDataSubservice : IEnemyDataSubservice
  {
    private EnemyManifestStaticData _manifest;

    private DictionaryData<EnemyTypeId, EnemyStaticData> _loadedEnemies = new();

    private readonly IAssetLoader _assetLoader;

    public EnemyDataSubservice(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    public async UniTask LoadSelfAsync() =>
      _manifest = await _assetLoader
        .LoadAsync<EnemyManifestStaticData>(StaticDataAddresses.EnemyManifestAddress);

    public async UniTask<EnemyStaticData> ForEnemyAsync(EnemyTypeId typeId)
    {
      if (_loadedEnemies.TryGetValue(typeId, out EnemyStaticData cached))
        return cached;

      _manifest.Enemies.TryGetValue(typeId, out AssetReferenceT<EnemyStaticData> entry);
      if (entry == null) return null;

      EnemyStaticData data = await _assetLoader.LoadAsync<EnemyStaticData>(entry);

      _loadedEnemies[typeId] = data;
      return data;
    }

    /// <summary>
    /// Loads the attack preset via Addressables.
    /// IAssetLoader caches the result by GUID — no matter how many enemies
    /// reference the same preset, it will only exist once in memory.
    /// </summary>
    public async UniTask<AttackPresetStaticData> ForAttackPresetAsync(EnemyStaticData enemyData)
    {
      if (enemyData.AttackPresetReference == null
        || !enemyData.AttackPresetReference.RuntimeKeyIsValid())
      {
        Debug.LogError($"[EnemyDataSubservice] AttackPresetReference is not set for enemy '{enemyData.EnemyTypeId}'");
        return null;
      }

      return await _assetLoader.LoadAsync<AttackPresetStaticData>(enemyData.AttackPresetReference);
    }
  }
}
