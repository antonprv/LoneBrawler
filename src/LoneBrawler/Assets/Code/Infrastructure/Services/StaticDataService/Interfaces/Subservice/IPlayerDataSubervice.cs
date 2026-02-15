// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IPlayerDataSubervice
  {
    public float AttackDamage { get; }
    public float AttackRadius { get; }
    public float AttackRange { get; }
    public int MaxEnemiesHit { get; }
    public float MaxHealth { get; }

    public Task LoadSelfAsync();
  }
}
