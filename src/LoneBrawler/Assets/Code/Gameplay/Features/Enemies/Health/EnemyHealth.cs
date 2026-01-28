// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Gameplay.Features.Common;
using Code.Gameplay.Features.Enemies.Animations;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyHealth : MonoBehaviour, IHealth, IActivatable
  {
    public EnemyAnimator animator;

    [field: SerializeField]
    public float MaxHealth { get; set; } = 50f;
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
    private bool _isActive;

    private void Awake()
    {
      Activate();
      CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
      if (CurrentHealth.IsNearlyZero() || !_isActive) return;

      CurrentHealth -= damage;
      animator.PlayHit();
    }

    public void Activate() => _isActive = true;

    public void Deactivate() => _isActive = false;
  }
}
