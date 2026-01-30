// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Data.SaveData;
using Code.Data.SaveData.Player;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Features.Player.Animations;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using TMPro;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Health
{
  [RequireComponent(typeof(PlayerAnimator))]
  public class PlayerHealth : MonoBehaviour, IHealth, IProgressReader, IProgressWriter
  {
    public event Action OnHealthChanged;

    public float MaxHealth
    {
      get => _state.MaxHealth;
      set
      {
        if (value == _state.MaxHealth) return;
        _state.MaxHealth = value;
      }
    }

    public float CurrentHealth
    {
      get => _state.CurrentHealth;
      set
      {
        if (value == _state.CurrentHealth) return;
        _state.CurrentHealth = value;
        OnHealthChanged?.Invoke();
      }
    }

    private PLayerState _state;
    private IAnimator _animator;

    public void Construct(IAnimator animator) => _animator = animator;

    public void TakeDamage(float damage)
    {
      if (CurrentHealth.IsNearlyZero()) return;

      CurrentHealth -= damage;
      _animator.PlayHit();
    }

    public void ReadProgress(GameProgress playerProgress)
    {
      _state = playerProgress.PLayerState;
      OnHealthChanged?.Invoke();
    }

    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.PLayerState.MaxHealth = MaxHealth;
      playerProgress.PLayerState.CurrentHealth = CurrentHealth;
    }
  }
}
