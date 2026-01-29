// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Linq;

using Code.Data.StaticData;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class EnemyDataSubservice : IEnemyDataSubservice
  {
    private Dictionary<EnemyTypeId, EnemyStaticData> _enemies;

    public void LoadEnemies() => _enemies = Resources
        .LoadAll<EnemyStaticData>("StaticData/Enemies")
        .ToDictionary(x => x.EnemyTypeId, x => x);

    public EnemyStaticData ForEnemy(EnemyTypeId typeId) =>
      _enemies.TryGetValue(typeId, out EnemyStaticData enemyStaticData)
      ? enemyStaticData
      : null;
  }
}
