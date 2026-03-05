// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile
{
  /// <summary>
  /// Ranged attack projectile. Works through a pool - never destroyed, returned instead.
  ///
  /// Lifecycle:
  ///   1. ProjectilePool.Get() → activate + call Launch()
  ///   2. Flies forward in Update()
  ///   3. Hit (OnTriggerEnter) or expired lifetime → Return to pool
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class EnemyProjectile : MonoBehaviour
  {
    [SerializeField] private float _lifetime = 5f;

    private IProjectilePool _pool;
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private int _layerMask;

    private bool _active;
    private float _timeAlive;

    // ──────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Called by RangedAttackBehaviour immediately after Get() from the pool.
    /// </summary>
    public void Launch(
      Vector3 direction,
      float speed,
      float damage,
      int playerLayerMask,
      IProjectilePool pool)
    {
      _pool = pool;
      _direction = direction.normalized;
      _speed = speed;
      _damage = damage;
      _layerMask = playerLayerMask;
      _timeAlive = 0f;
      _active = true;
    }

    // ──────────────────────────────────────────────
    //  Unity lifecycle
    // ──────────────────────────────────────────────

    private void Update()
    {
      if (!_active) return;

      transform.position += _direction * (_speed * Time.deltaTime);

      _timeAlive += Time.deltaTime;
      if (_timeAlive >= _lifetime)
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!_active) return;
      if (((1 << other.gameObject.layer) & _layerMask) == 0) return;

      other.GetComponent<IHealth>()?.TakeDamage(_damage);
      ReturnToPool();
    }

    private void OnDisable()
    {
      // Reset flag on deactivation - safe for reuse
      _active = false;
    }

    // ──────────────────────────────────────────────
    //  Private
    // ──────────────────────────────────────────────

    private void ReturnToPool()
    {
      _active = false;
      _pool?.Return(this);
    }
  }
}
