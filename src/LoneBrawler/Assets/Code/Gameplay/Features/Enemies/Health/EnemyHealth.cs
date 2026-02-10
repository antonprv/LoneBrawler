// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.CustomTypes;
using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.Animations;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyHealth : MonoBehaviour, IEnemyHealth
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

    public void SetValues(EnemyStaticData staticData)
    {
      MaxHealth = staticData.MaxHealth;
      CurrentHealth = staticData.MaxHealth;
    }

    public void Construct(IAnimator animator)
    {
      _animator = animator;
    }

    public event Action OnHealthChanged;

    private float _currentHealth;
    private IAnimator _animator;

    public void TakeDamage(float damage)
    {
      if (CurrentHealth.IsNearlyZero()) return;

      CurrentHealth -= damage;
      _animator.PlayHit();
    }

  }
}
