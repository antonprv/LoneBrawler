// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Configs;

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.


namespace Code.Data
{
  [Serializable]
  public class PlayerStats : IValidatableData
  {
    public float Damage;
    public float Range;
    public float Radius;
    public int MaxEnemiesHit;

    public PlayerStats()
    {
      Damage = GameConfiguration.PlayerAttackDamage;
      Range = GameConfiguration.PlayerAttackRange;
      Radius = GameConfiguration.PlayerAttackRadius;
      MaxEnemiesHit = GameConfiguration.PlayerMaxEnemiesHit;
    }

    public bool IsDataNull()
    {
      return Damage != 0
        && Range != 0
        && Radius != 0
        && MaxEnemiesHit != 0;
    }
  }
}
