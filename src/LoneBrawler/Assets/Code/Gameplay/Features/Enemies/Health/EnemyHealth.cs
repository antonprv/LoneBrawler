// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Features.Enemies.Animations;

using TMPro;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyHealth : MonoBehaviour, IHealth
  {
    public float MaxHealth { get; set; }
    public float CurrentHealth
    {
      get => _currentHealth;
      set
      {
        if (value == _currentHealth) return;
        _currentHealth = value;
        OnHealthChanged?.Invoke();
      }
    }


    public event Action OnHealthChanged;

    private float _currentHealth;
    private IAnimator _animator;
    public void Construct(IAnimator animator)
    {
      _animator = animator;
    }

    public void TakeDamage(float damage)
    {
      if (CurrentHealth.IsNearlyZero()) return;

      CurrentHealth -= damage;
      _animator.PlayHit();
    }
  }
}
