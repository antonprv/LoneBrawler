// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig
{
  /// <summary>
  /// Ranged attack: on OnCast() retrieves a projectile from the pool and launches it.
  /// PerformHit() is not needed — damage is applied by the projectile on impact.
  /// </summary>
  public class RangedAttackBehaviour : IAttackBehaviour
  {
    private const int DefaultPoolSize = 8;

    private Transform _owner;
    private AttackPresetStaticData _preset;
    private int _layerMask;
    private IProjectilePool _pool;

    public void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,        // not used directly — damage is dealt by the projectile
      int playerLayerMask)
    {
      _owner = owner;
      _preset = preset;
      _layerMask = playerLayerMask;
    }

    /// <summary>
    /// Overload with a pre-loaded prefab — called from AttackBehaviourFactory.
    /// The pool is created here.
    /// </summary>
    public void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask,
      GameObject loadedProjectilePrefab,
      int poolSize = DefaultPoolSize)
    {
      Initialize(owner, preset, playerHealth, playerLayerMask);

      if (loadedProjectilePrefab != null)
        _pool = new ProjectilePool(loadedProjectilePrefab, poolSize);
      else
      {
        string presetName = preset != null ? preset.PresetId : "unknown";
        Debug.LogWarning($"[RangedAttackBehaviour] ProjectilePrefab not loaded for preset '{presetName}'");
      }
    }

    /// <summary>Damage is dealt by the projectile — nothing to do here.</summary>
    public void PerformHit() { }

    /// <summary>Cast moment: retrieve a projectile from the pool and launch it.</summary>
    public void OnCast()
    {
      if (_pool == null)
      {
        Debug.LogWarning($"[RangedAttackBehaviour] Pool is not initialized (preset: '{_preset?.PresetId}')");
        return;
      }

      Vector3 spawnPos = _owner.position + Vector3.up * 1f + _owner.forward * 0.5f;

      EnemyProjectile projectile = _pool.Get(spawnPos, _owner.rotation);
      projectile.Launch(_owner.forward, _preset.ProjectileSpeed, _preset.Damage, _layerMask, _pool);
    }

    public void OnAttackEnded() { }
  }
}
