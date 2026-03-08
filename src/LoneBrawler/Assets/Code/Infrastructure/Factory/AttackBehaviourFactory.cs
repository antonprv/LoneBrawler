// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Attack;
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
  /// The preset is passed in already loaded from EnemyDataSubservice -
  /// this factory is not responsible for loading StaticData assets.
  ///
  /// To add a new attack type - register a new case here.
  /// </summary>
  public class AttackBehaviourFactory : IAttackBehaviourFactory
  {
    private readonly IGameLog _logger;

    public AttackBehaviourFactory(IGameLog gameLog) => _logger = gameLog;

    public async UniTask<IAttackBehaviour> CreateAsync(
      Transform ownerTransform,
      EnemyStaticData staticData,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask,
      IAssetLoader assetLoader)
    {
      if (staticData.IsContainer) return null;

      switch (staticData.EnemyAttackType)
      {
        case EnemyAttackType.Melee:
          {
            var behaviour = new MeleeAttackBehaviour();

            GameObject hitVfxPrefab = await TryLoadVfx(
              preset.HitVfxPrefab, preset.PresetId, "HitVfx", assetLoader);

            behaviour.Initialize(ownerTransform, preset, playerHealth, playerLayerMask, hitVfxPrefab);
            return behaviour;
          }

        case EnemyAttackType.Ranged:
          {
            var behaviour = new RangedAttackBehaviour();

            (GameObject projectilePrefab, GameObject castVfxPrefab, GameObject hitVfxPrefab) =
              await UniTask.WhenAll(
                TryLoadPrefab(preset.ProjectilePrefab, preset.PresetId, "ProjectilePrefab", assetLoader),
                TryLoadVfx(preset.CastVfxPrefab, preset.PresetId, "CastVfx", assetLoader),
                TryLoadVfx(preset.HitVfxPrefab, preset.PresetId, "HitVfx", assetLoader)
              );

            if (projectilePrefab == null)
              _logger.Log(LogType.Warning,
                $"[AttackBehaviourFactory] ProjectilePrefab was not assigned for ranged attack preset " +
                $"'{preset.PresetId}' of {staticData.EnemyTypeId}");

            behaviour.Initialize(
              ownerTransform, preset, playerHealth, playerLayerMask,
              projectilePrefab, castVfxPrefab, hitVfxPrefab);

            return behaviour;
          }

        default:
          _logger.Log(LogType.Error,
            $"[AttackBehaviourFactory] Unknown EnemyAttackType: {staticData.EnemyAttackType}");
          return null;
      }
    }

    // ──────────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────────

    private async UniTask<GameObject> TryLoadPrefab(
      UnityEngine.AddressableAssets.AssetReferenceGameObject reference,
      string presetId,
      string fieldName,
      IAssetLoader assetLoader)
    {
      if (reference != null && reference.RuntimeKeyIsValid())
        return await assetLoader.LoadAsync<GameObject>(reference);

      return null;
    }

    private async UniTask<GameObject> TryLoadVfx(
      UnityEngine.AddressableAssets.AssetReferenceGameObject reference,
      string presetId,
      string fieldName,
      IAssetLoader assetLoader)
    {
      if (reference != null && reference.RuntimeKeyIsValid())
        return await assetLoader.LoadAsync<GameObject>(reference);

      return null;  // VFX is optional - null means no effect
    }
  }
}
