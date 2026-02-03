// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData.SecondaryData;
using Code.Data.StaticData.Types;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class LevelDataSubservice : ILevelDataSubservice
  {
    private Dictionary<EnemyTypeId, EnemySpawnerStaticData> _enemies;

    //public void LoadEnemies() => _enemies = Resources
    //    .LoadAll<EnemySpawnerStaticData>("StaticData/Levels")
    //    .ToDictionary(x => x.EnemyTypeId, x => x);

    //public EnemyStaticData ForEnemy(EnemyTypeId typeId) =>
    //  _enemies.TryGetValue(typeId, out EnemyStaticData enemyStaticData)
    //  ? enemyStaticData
    //  : null;
  }
}
