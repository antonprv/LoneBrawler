// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data;
using Code.Data.DataExtensions;
using Code.Infrastructure.Services.PersistentProgress;

using UnityEngine;

namespace Code.Gameplay.Features.Player
{
  [RequireComponent(typeof(PlayerAnimator))]
  public class PlayerHealth : MonoBehaviour, IProgressReader, IProgressWriter
  {
    public event Action OnHealthChanged;

    public PlayerAnimator animator;

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

    public void TakeDamage(float damage)
    {
      if (CurrentHealth.IsNearlyZero()) return;

      CurrentHealth -= damage;
      animator.PlayHit();
      OnHealthChanged?.Invoke();
    }

    public void ReadProgress(PlayerProgress playerProgress)
    {
      _state = playerProgress.PLayerState;
      OnHealthChanged?.Invoke();
    }

    public void WriteToProgress(PlayerProgress playerProgress)
    {
      playerProgress.PLayerState.MaxHealth = MaxHealth;
      playerProgress.PLayerState.CurrentHealth = CurrentHealth;
    }
  }
}
