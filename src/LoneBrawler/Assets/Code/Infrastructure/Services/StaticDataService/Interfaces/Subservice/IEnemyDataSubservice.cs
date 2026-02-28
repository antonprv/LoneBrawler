// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Enemies;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IEnemyDataSubservice
  {
    UniTask LoadSelfAsync();
    UniTask<EnemyStaticData> ForEnemyAsync(EnemyTypeId typeId);

    /// <summary>
    /// Loads the attack preset referenced in EnemyStaticData.
    /// AssetLoader caches by GUID — repeated calls with the same preset
    /// return the same in-memory instance without re-loading.
    /// </summary>
    UniTask<AttackPresetStaticData> ForAttackPresetAsync(EnemyStaticData enemyData);
  }
}
