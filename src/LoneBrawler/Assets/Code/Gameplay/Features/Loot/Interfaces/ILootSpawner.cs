// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Enemies;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILootSpawner
  {
    public void Construct(
      string spawnerId,
      EnemyTypeId enemyTypeId
      );

    public UniTaskVoid SpawnLoot(Vector3 position);
  }
}
