// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Infrastructure.Factory.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
  public class LootSpawner : MonoBehaviour
  {
    private IGameFactory _gameFactory;
    private IEnemyDeath _enemyDeath;

    public void Construct(IGameFactory gameFactory, IEnemyDeath enemyDeath)
    {
      _gameFactory = gameFactory;
      _enemyDeath = enemyDeath;

      _enemyDeath.OnDead += SpawnLoot;
    }

    private void SpawnLoot()
    {
      _enemyDeath.OnDead -= SpawnLoot;
      _gameFactory.CreateLoot(gameObject.transform);
    }
  }
}
