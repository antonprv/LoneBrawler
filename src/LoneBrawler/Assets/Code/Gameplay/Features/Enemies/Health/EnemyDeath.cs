// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.DataExtensions;
using Code.Data.StaticData;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Enemies.Health.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyDeath : MonoBehaviour, IEnemyDeath
  {
    public GameObject DeathFX;

    public bool IsDead { get; private set; }
    private float _disappearDelay;

    private IGameLog _logger;
    private IAnimator _animator;
    private IHealth _health;
    private GameObject _spawnedDeathFX;

    public event Action OnDead;

    public void SetValues(EnemyStaticData staticData)
    {
      _disappearDelay = staticData.DisappearDelay;
    }

    public void Construct(IAnimator animator, IHealth health)
    {
      IsDead = false;
      _logger = RootContext.Resolve<IGameLog>();

      _animator = animator;

      _health = health;
      _health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy() =>
      _health.OnHealthChanged -= HandleHealthChanged;

    private void HandleHealthChanged()
    {
      if (_health.CurrentHealth.IsNearlyZero())
        Die();
    }

    private void Die()
    {
      DeactivateComponents();

      _animator.PlayDeath();

      _spawnedDeathFX = Instantiate(
        DeathFX,
        transform.position,
        Quaternion.identity
        );

      OnDead?.Invoke();

      IsDead = true;

      StartCoroutine(DespawnEnemy());
    }

    private IEnumerator DespawnEnemy()
    {
      yield return new WaitForSeconds(_disappearDelay);

      _logger.Log("Destroying enemy...");
      Destroy(_spawnedDeathFX);
      Destroy(gameObject);
    }

    private void DeactivateComponents()
    {
      foreach (var component in gameObject.GetComponents<IDeactivatable>())
        component.Deactivate();
    }
  }
}
