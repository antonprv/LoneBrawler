// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Configs;

using Code.Data.SaveData.Common.Interfaces;

namespace Code.Data.SaveData.Player
{
  [Serializable]
  public class PLayerState : IValidatableData
  {
    public float MaxHealth;
    public float CurrentHealth;

    /// <summary>
    /// Empty constructor will always create invalid (empty) PlayerState
    /// </summary>
    public PLayerState()
    {
      MaxHealth = GameConfiguration.PlayerMaxHealth;
      CurrentHealth = MaxHealth;
    }

    public PLayerState(float maxHealth, float currentHealth)
    {
      MaxHealth = maxHealth;
      CurrentHealth = currentHealth;
    }

    public bool IsDataNull()
    {
      return MaxHealth != 0 && CurrentHealth != 0;
    }
  }
}
