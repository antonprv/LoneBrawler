// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.VectorTypes.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Data.SaveData.Player
{
  [System.Serializable]
  public class PlayerStats : IValidatableData
  {
    public float MovementSpeed;
    public float RotationSpeed;

    public float Damage;
    public float Range;
    public float Radius;

    public int MaxEnemiesHit;

    public PlayerStats(IPlayerDataSubervice playerStaticData)
    {
      MovementSpeed = playerStaticData.MovementSpeed;
      RotationSpeed = playerStaticData.RotationSpeed;

      Damage = playerStaticData.AttackDamage;
      Range = playerStaticData.AttackRange;
      Radius = playerStaticData.AttackRadius;

      MaxEnemiesHit = playerStaticData.MaxEnemiesHit;
    }

    public bool IsValid()
    {
      return Damage != 0
        && Range != 0
        && Radius != 0
        && MaxEnemiesHit != 0;
    }
  }
}
