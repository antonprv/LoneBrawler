// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace Code.Data.StaticData.Types
{
  [Serializable]
  public class EnemySpawnerData
  {
    public string SpawnerId;
    public EnemyTypeId EnemyTypeId;
    public Vector3 Position;

    public EnemySpawnerData(
      string spawnerId,
      EnemyTypeId enemyTypeId,
      Vector3 position
      )
    {
      SpawnerId = spawnerId;
      EnemyTypeId = enemyTypeId;
      Position = position;
    }
  }
}
