// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Factory
{

  /// <summary>
  /// Creates the appropriate IAttackBehaviour based on enemy data.
  /// The preset is passed in already loaded from EnemyDataSubservice —
  /// this factory is not responsible for loading StaticData assets.
  ///
  /// To add a new attack type — register a new case here.
  /// </summary>
  public class AttackBehaviourFactory : IAttackBehaviourFactory
  {
    public async UniTask<IAttackBehaviour> CreateAsync(
      Transform ownerTransform,
      EnemyStaticData staticData,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask,
      IAssetLoader assetLoader)
    {
      switch (staticData.EnemyAttackType)
      {
        case EnemyAttackType.Melee:
          {
            var behaviour = new MeleeAttackBehaviour();
            behaviour.Initialize(ownerTransform, preset, playerHealth, playerLayerMask);
            return behaviour;
          }

        case EnemyAttackType.Ranged:
          {
            var behaviour = new RangedAttackBehaviour();

            GameObject projectilePrefab = null;
            if (preset.ProjectilePrefab != null)
              projectilePrefab = await assetLoader.LoadAsync<GameObject>(preset.ProjectilePrefab);

            behaviour.Initialize(ownerTransform, preset, playerHealth, playerLayerMask, projectilePrefab);
            return behaviour;
          }

        default:
          Debug.LogError($"[AttackBehaviourFactory] Unknown EnemyAttackType: {staticData.EnemyAttackType}");
          return null;
      }
    }
  }
}
