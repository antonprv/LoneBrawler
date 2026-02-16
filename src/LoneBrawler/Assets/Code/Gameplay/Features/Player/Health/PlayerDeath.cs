// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Features.Player.Movement;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

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
