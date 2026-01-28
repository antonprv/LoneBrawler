// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;
using Code.Gameplay.Features.Common;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Enemies.Movement;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  [RequireComponent(typeof(IMovableAgent))]
  public class EnemyDeath : MonoBehaviour
  {
    public EnemyAnimator animator;

    public GameObject DeathFX;
    private IHealth _health;
    private IMovableAgent _move;

    private void Awake()
    {
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
    }

    private void DeactivateComponents()
    {
      foreach (var component in gameObject.GetComponents<IDeactivatable>())
        component.Deactivate();
    }
  }
}
