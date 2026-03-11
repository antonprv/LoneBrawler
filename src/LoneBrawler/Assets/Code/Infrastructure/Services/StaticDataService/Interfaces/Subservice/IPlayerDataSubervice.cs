// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IPlayerDataSubervice
  {
    public float MaxHealth { get; }

    public float AttackDamage { get; }
    public float AttackRadius { get; }
    public float AttackRange { get; }
    public int MaxEnemiesHit { get; }

    float MovementSpeed { get; }
    float RotationSpeed { get; }
    float DeathDelay { get; }

    public UniTask LoadSelfAsync();
  }
}
