// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces
{
  public interface IEnemyDataSubservice
  {
    EnemyStaticData ForEnemy(EnemyTypeId typeId);
    void LoadEnemies();
  }
}
