// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;

using Code.Infrastructure.Factory.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILootSpawner
  {
    public void Construct(
      IGameFactory gameFactory,
      EnemyTypeId typeId,
      string id
      );

    public void SpawnLoot(Vector3 position);
  }
}
