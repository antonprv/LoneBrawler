// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IGameConfigSubservice
  {
    int PlayerCollision { get; }
    string PlayerStartTag { get; }
    string PlayerTag { get; }

    string EnemySpawnerTag { get; }

    int EnemyHitableLayer { get; }
  }
}
