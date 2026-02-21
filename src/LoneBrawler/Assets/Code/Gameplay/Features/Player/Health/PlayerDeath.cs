// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.FastMath;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Features.Player.Movement;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

using R3;

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

    private CompositeDisposable _disposables;

    public void Construct(IAnimator animator, IHealth health)
    {
      _disposables = new CompositeDisposable();

      IsDead = false;

      _animator = animator;
      _health = health;

      SubscribeToRP();

    }

    private void SubscribeToRP()
    {
      _health.CurrentHealthRP
        .Skip(1)
        .Where(hp => hp.IsNearlyZero())
        .Subscribe(_ => Die())
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables?.Dispose();

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
