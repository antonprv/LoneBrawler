// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Data.SaveData;
using Code.Data.SaveData.Player;
using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
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
    public SoundPlayer soundPlayer;

    private readonly ReactiveProperty<float> _currentHealthRP = new(0f);
    private readonly ReactiveProperty<float> _maxHealthRP = new(0f);

    public ReadOnlyReactiveProperty<float> CurrentHealthRP => _currentHealthRP;
    public ReadOnlyReactiveProperty<float> MaxHealthRP => _maxHealthRP;

    // Damage multiplier received. 1f - normal, 0f - full immunity.
    private float _damageModifier = 1f;

    // When true - TakeDamage is completely ignored (GodBuff).
    private bool _isInvulnerable;

    private IAnimator _animator;

    public void Construct(IAnimator animator) => _animator = animator;

    public void TakeDamage(float damage)
    {
      if (_isInvulnerable) return;
      if (_currentHealthRP.Value.IsNearlyZero()) return;

      _currentHealthRP.Value -= damage * _damageModifier;
      _animator.PlayHit();
      soundPlayer.PlaySound(SoundType.Hit);
    }

    /// <summary>
    /// Recovers health by the specified amount without exceeding the maximum.
    /// </summary>
    public void Heal(float amount)
    {
      _currentHealthRP.Value =
        Mathf.Min(_currentHealthRP.Value + amount, _maxHealthRP.Value);
    }

    /// <summary>
    /// Permanently increases maximum health.
    /// </summary>
    public void AddMaxHealth(float amount)
    {
      _maxHealthRP.Value += amount;
    }

    /// <summary>
    /// Sets the damage multiplier received (0..1).
    /// Accumulates on multiple calls - each call multiplies by delta.
    /// </summary>
    public void ApplyDamageModifier(float modifier) =>
      _damageModifier *= modifier;

    /// <summary>
    /// Removes previously applied damage multiplier.
    /// </summary>
    public void RemoveDamageModifier(float modifier)
    {
      if (modifier.IsNearlyZero()) return;
      _damageModifier /= modifier;
    }

    /// <summary>
    /// Enables/disables complete immunity to damage.
    /// </summary>
    public void SetInvulnerable(bool value) =>
      _isInvulnerable = value;

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
