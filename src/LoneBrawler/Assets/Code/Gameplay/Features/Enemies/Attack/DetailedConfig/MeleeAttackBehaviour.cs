// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Attack;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig
{
  /// <summary>
  /// Melee attack: OverlapSphere around a point in front of the enemy.
  /// Supports SingleTarget (first hit) and AoE (all targets within radius).
  /// Spawns HitVfx at the hit position via pool.
  /// </summary>
  public class MeleeAttackBehaviour : IAttackBehaviour
  {
    private Transform _owner;
    private AttackPresetStaticData _preset;
    private IHealth _playerHealth;
    private int _layerMask;
    private IVfxPool _hitVfxPool;  // null if no VFX assigned

    private Collider[] _hits;

    public void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask)
    {
      _owner = owner;
      _preset = preset;
      _playerHealth = playerHealth;
      _layerMask = playerLayerMask;

      int bufferSize = preset.TargetMode == AttackTargetMode.AreaOfEffect ? 10 : 1;
      _hits = new Collider[bufferSize];
    }

    /// <summary>
    /// Overload with pre-loaded VFX prefab - called from AttackBehaviourFactory.
    /// </summary>
    public void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask,
      GameObject hitVfxPrefab,
      int vfxPoolSize = 4)
    {
      Initialize(owner, preset, playerHealth, playerLayerMask);

      if (hitVfxPrefab != null)
        _hitVfxPool = new Vfx.VfxPool(hitVfxPrefab, vfxPoolSize);
    }

    public void PerformHit()
    {
      float radius = _preset.TargetMode == AttackTargetMode.AreaOfEffect
        ? _preset.AreaRadius
        : _preset.AttackStartRange;

      Vector3 hitPos = GetHitPosition();
      int hitCount = Physics.OverlapSphereNonAlloc(hitPos, radius, _hits, _layerMask);

      if (hitCount == 0) return;

      _hitVfxPool?.Get(hitPos, _owner.rotation);

      if (_preset.TargetMode == AttackTargetMode.SingleTarget)
      {
        if (_playerHealth != null)
          _playerHealth.TakeDamage(_preset.Damage);
      }
      else
      {
        for (int i = 0; i < hitCount; i++)
        {
          if (_hits[i].TryGetComponent<IHealth>(out var health))
            health.TakeDamage(_preset.Damage);
        }
      }
    }

    public void OnCast() { /* melee attack - no cast needed */ }
    public void OnAttackEnded() { }

    private Vector3 GetHitPosition() =>
      _owner.position + Vector3.up * 0.5f + _owner.forward * _preset.AttackStartRange;
  }
}
