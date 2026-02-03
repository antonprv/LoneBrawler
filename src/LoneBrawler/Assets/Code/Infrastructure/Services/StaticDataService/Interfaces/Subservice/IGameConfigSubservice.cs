// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IGameConfigSubservice
  {
    string PlayerStartTag { get; }

    int PlayerCollision { get; }
    int EnemyHitableLayer { get; }
    int LootLayer { get; }
  }
}
