// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.SaveData.Common.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Data.SaveData.Player
{
  [Serializable]
  public class PlayerStats : IValidatableData
  {
    public float Damage;
    public float Range;
    public float Radius;
    public int MaxEnemiesHit;

    public PlayerStats(IPlayerDataSubervice playerStaticData)
    {
      Damage = playerStaticData.AttackDamage;
      Range = playerStaticData.AttackRange;
      Radius = playerStaticData.AttackRadius;
      MaxEnemiesHit = playerStaticData.MaxEnemiesHit;
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
