// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IEnemyDataSubservice
  {
    void LoadSelf();

    EnemyStaticData ForEnemy(EnemyTypeId typeId);
  }
}
