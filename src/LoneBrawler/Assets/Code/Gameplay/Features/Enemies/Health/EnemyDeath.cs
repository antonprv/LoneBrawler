// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  [RequireComponent(typeof(IMovableAgent))]
  public class EnemyDeath : MonoBehaviour, IEnemyDeath
  {
    public GameObject DeathFX;

    public bool IsDead { get; private set; }
    public float DisappearDelay { get; set; }

    private IGameLog _logger;
    private IAnimator _animator;
    private IHealth _health;

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

      Instantiate(
        DeathFX,
        transform.position,
        Quaternion.identity
        );

      IsDead = true;

      StartCoroutine(DespawnEnemy());
    }

    private IEnumerator DespawnEnemy()
    {
      yield return new WaitForSeconds(DisappearDelay);

      _logger.Log("Destroying enemy...");
      Destroy(gameObject);
    }

    private void DeactivateComponents()
    {
      foreach (var component in gameObject.GetComponents<IDeactivatable>())
        component.Deactivate();
    }
  }
}
