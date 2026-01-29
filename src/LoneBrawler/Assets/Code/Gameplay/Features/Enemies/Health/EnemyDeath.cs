// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.NPCInterfaces;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  [RequireComponent(typeof(IMovableAgent))]
  public class EnemyDeath : MonoBehaviour
  {
    public EnemyAnimator animator;

    public GameObject DeathFX;
    private IGameLog _logger;
    private IStaticDataService _staticDataService;
    private IHealth _health;
    private IMovableAgent _move;

    private void Awake()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();

      _health = GetComponent<IHealth>();
      _move = GetComponent<IMovableAgent>();
    }

    private void Start() =>
      _health.OnHealthChanged += HandleHealthChanged;

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

      animator.PlayDeath();

      Instantiate(
        DeathFX,
        transform.position,
        Quaternion.identity
        );

      StartCoroutine(DespawnEnemy());
    }

    private IEnumerator DespawnEnemy()
    {
      yield return new WaitForSeconds(
        _staticDataService.GameConfig.EnemyDisappearDelay
        );

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
