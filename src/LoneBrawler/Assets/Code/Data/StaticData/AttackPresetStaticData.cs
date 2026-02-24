// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs.Types;
using Code.Data.StaticData.Types;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  /// <summary>
  /// Reusable attack preset ScriptableObject.
  /// Created once (e.g. "Fireball", "HammerSwing") and assigned to EnemyStaticData.
  /// </summary>
  [CreateAssetMenu(fileName = "AttackPreset", menuName = "StaticData/Attacks/AttackPreset")]
  public class AttackPresetStaticData : ScriptableObject
  {
    [Header("Identity")]
    public string PresetId;

    [Header("Attack Mode")]
    [NoNone] public AttackTargetMode TargetMode = AttackTargetMode.SingleTarget;

    [Header("Timing")]
    [Range(0f, 10f)] public float WindupDuration = 0.3f;   // time before the hit lands (windup animation)
    [Range(0f, 10f)] public float HitWindowDuration = 0.1f;  // window during which the hit is registered
    [Range(0f, 10f)] public float RecoveryDuration = 0.5f;  // recovery time after the hit

    [Header("Damage & Range")]
    [Range(1f, 999f)] public float Damage = 20f;
    [Range(0.1f, 30f)] public float Range = 1.5f;   // melee: overlap sphere radius; ranged: projectile spawn distance
    [Range(0f, 20f)] public float AreaRadius = 0f;     // >0 for AoE attacks only

    [Header("Ranged Projectile")]
    public AssetReferenceGameObject ProjectilePrefab;      // null means melee / no projectile
    [Range(1f, 100f)] public float ProjectileSpeed = 15f;

    [Header("Visual / Audio (optional)")]
    public AssetReferenceGameObject HitVfxPrefab;
    public AssetReferenceGameObject CastVfxPrefab;
  }
}
