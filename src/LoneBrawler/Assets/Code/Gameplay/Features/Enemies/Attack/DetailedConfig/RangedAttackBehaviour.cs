// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig
{
  /// <summary>
  /// Ranged attack: on PerformHit() (shared animation event with melee) spawns a projectile via OnCast().
  /// CastVfx is spawned at the moment of cast; HitVfx is spawned by the projectile on impact.
  /// </summary>
  public class RangedAttackBehaviour : IAttackBehaviour
  {
    private const int DefaultPoolSize = 8;
    private const int DefaultVfxPoolSize = 4;

    private Transform _owner;
    private AttackPresetStaticData _preset;
    private int _layerMask;
    private IProjectilePool _pool;
    private IVfxPool _castVfxPool;  // null if no VFX assigned
    private IVfxPool _hitVfxPool;   // passed through to projectile

    public void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,        // not used directly - damage is dealt by the projectile
      int playerLayerMask)
    {
      _owner = owner;
      _preset = preset;
      _layerMask = playerLayerMask;
    }

    /// <summary>
    /// Overload with pre-loaded prefabs - called from AttackBehaviourFactory.
    /// </summary>
    public void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask,
      GameObject loadedProjectilePrefab,
      GameObject castVfxPrefab,
      GameObject hitVfxPrefab,
      int poolSize = DefaultPoolSize)
    {
      Initialize(owner, preset, playerHealth, playerLayerMask);

      if (loadedProjectilePrefab != null)
        _pool = new ProjectilePool(loadedProjectilePrefab, poolSize);
      else
        Debug.LogWarning($"[RangedAttackBehaviour] ProjectilePrefab not loaded for preset '{preset?.PresetId}'");

      if (castVfxPrefab != null)
        _castVfxPool = new VfxPool(castVfxPrefab, DefaultVfxPoolSize);

      if (hitVfxPrefab != null)
        _hitVfxPool = new VfxPool(hitVfxPrefab, DefaultVfxPoolSize);
    }

    /// <summary>
    /// Called from the shared OnPointAttackHit animation event.
    /// For ranged enemies this is the cast moment - spawn a projectile.
    /// </summary>
    public void PerformHit() => OnCast();

    /// <summary>Cast: spawn cast VFX, retrieve a projectile from the pool and launch it.</summary>
    public void OnCast()
    {
      if (_pool == null)
      {
        Debug.LogWarning($"[RangedAttackBehaviour] Pool is not initialized (preset: '{_preset?.PresetId}')");
        return;
      }

      Vector3 spawnPos = _owner.position + Vector3.up * 1f + _owner.forward * 0.5f;

      _castVfxPool?.Get(spawnPos, _owner.rotation);

      EnemyProjectile projectile = _pool.Get(spawnPos, _owner.rotation);
      projectile.Launch(_owner.forward, _preset.ProjectileSpeed, _preset.Damage, _layerMask, _pool, _hitVfxPool);
    }

    public void OnAttackEnded() { }
  }
}
