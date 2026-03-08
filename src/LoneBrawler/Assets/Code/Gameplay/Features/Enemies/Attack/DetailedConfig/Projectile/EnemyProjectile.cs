// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx.Interfaces;
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
  ///   3. Hit (OnTriggerEnter) or expired lifetime → spawn HitVfx → Return to pool
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class EnemyProjectile : MonoBehaviour
  {
    public SoundPlayer soundPlayer;

    public float lifetime = 5f;

    private IProjectilePool _pool;
    private IVfxPool _hitVfxPool;
    private Vector3 _direction;
    private float _speed;
    private float _damage;
    private int _layerMask;

    private bool _active;
    private float _timeAlive;

    // ──────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────

    public void Launch(
      Vector3 direction,
      float speed,
      float damage,
      int playerLayerMask,
      IProjectilePool pool,
      IVfxPool hitVfxPool = null)
    {
      _pool = pool;
      _hitVfxPool = hitVfxPool;
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
      if (_timeAlive >= lifetime)
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!_active) return;
      if (((1 << other.gameObject.layer) & _layerMask) == 0) return;

      // Disable first - prevents a second OnTriggerEnter on adjacent colliders
      // from passing the _active check before ReturnToPool deactivates the object
      _active = false;

      other.GetComponent<IHealth>()?.TakeDamage(_damage);
      _hitVfxPool?.Get(transform.position, transform.rotation);

      soundPlayer.PlaySound(SoundType.Hit);

      _pool?.Return(this);
    }

    private void OnDisable() => _active = false;

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
