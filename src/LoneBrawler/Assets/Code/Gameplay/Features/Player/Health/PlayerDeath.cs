// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;
using Code.Gameplay.Features.Common;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Features.Player.Movement;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Health
{
  [RequireComponent(typeof(PlayerAnimator))]
  [RequireComponent(typeof(PlayerMove))]
  public class PlayerDeath : MonoBehaviour
  {
    public PlayerAnimator animator;

    public bool IsDead { get; private set; }

    public GameObject DeathFX;

    private IHealth _health;

    private void Awake()
    {
      _health = GetComponent<IHealth>();

      IsDead = false;
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
      IsDead = true;

      Instantiate(
        DeathFX,
        transform.position,
        Quaternion.identity
        );
    }

    private void DeactivateComponents()
    {
      foreach (var component in GetComponents<IDeactivatable>())
        component.Deactivate();
    }
  }
}
