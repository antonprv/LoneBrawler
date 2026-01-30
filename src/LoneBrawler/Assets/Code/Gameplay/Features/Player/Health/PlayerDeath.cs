// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Features.Player.Movement;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Health
{
  [RequireComponent(typeof(PlayerAnimator))]
  [RequireComponent(typeof(PlayerMove))]
  public class PlayerDeath : MonoBehaviour, IDeath
  {
    public bool IsDead { get; private set; }

    private IAnimator _animator;
    public GameObject DeathFX;

    private IHealth _health;

    public void Construct(IAnimator animator, IHealth health)
    {
      IsDead = false;

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
