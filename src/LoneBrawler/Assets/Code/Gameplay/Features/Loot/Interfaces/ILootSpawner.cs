// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Enemies;

using Code.Infrastructure.Factory.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILootSpawner
  {
    public void Construct(
      IGameFactory gameFactory,
      string spawnerId,
      EnemyTypeId enemyTypeId
      );

    public void SpawnLoot(Vector3 position);
  }
}
