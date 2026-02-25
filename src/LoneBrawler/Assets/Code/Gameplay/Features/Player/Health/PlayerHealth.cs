// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Data.SaveData;
using Code.Data.SaveData.Player;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using R3;

using UnityEngine;

using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Player.Health
{
  [RequireComponent(typeof(PlayerAnimator))]
  public class PlayerHealth : ZenjexBehaviour, IHealth, IProgressReader, IProgressWriter
  {
    private ReactiveProperty<float> _currentHealthRP = new(0f);
    private ReactiveProperty<float> _maxHealthRP = new(0f);

    public ReadOnlyReactiveProperty<float> CurrentHealthRP => _currentHealthRP;
    public ReadOnlyReactiveProperty<float> MaxHealthRP => _maxHealthRP;

    private IAnimator _animator;

    public void Construct(IAnimator animator) => _animator = animator;

    public void TakeDamage(float damage)
    {
      if (_currentHealthRP.Value.IsNearlyZero()) return;

      _currentHealthRP.Value -= damage;
      _animator.PlayHit();
    }

    public void ReadProgress(GameProgress playerProgress)
    {
      PLayerState state = playerProgress.PLayerState;
      _currentHealthRP.Value = state.CurrentHealth;
      _maxHealthRP.Value = state.MaxHealth;
    }

    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.PLayerState.MaxHealth = _maxHealthRP.Value;
      playerProgress.PLayerState.CurrentHealth = _currentHealthRP.Value;
    }
  }
}
