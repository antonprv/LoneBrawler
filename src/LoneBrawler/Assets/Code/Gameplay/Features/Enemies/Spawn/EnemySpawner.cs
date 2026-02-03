// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.SaveData;
using Code.Data.StaticData.Types;
using Code.Gameplay.Common;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Spawn
{
  public class EnemySpawner : MonoBehaviour, IProgressReader, IProgressWriter
  {
    public EnemyTypeId enemyTypeId;

    private bool _slain;

    private IGameFactory _gameFactory;
    private ILootSpawner _lootSpawner;
    private string _id;
    private GameObject _enemyObject;
    private IEnemyDeath _enemyDeath;

    public void Construct(
      IGameFactory gameFactory,
      ILootSpawner lootSpawner)
    {
      _gameFactory = gameFactory;
      _lootSpawner = lootSpawner;

      _id = GetComponent<UniqueId>().id;
      _lootSpawner.Construct(
        _gameFactory, enemyTypeId, _id);
    }

    public void ReadProgress(GameProgress playerProgress)
    {
      if (playerProgress.EnemiesKilled.ClearedSpawners.Contains(_id))
        _slain = true;
      else
      {
        Spawn();
      }
    }

    private void Spawn()
    {
      _enemyObject = _gameFactory.CreateEnemy(enemyTypeId, gameObject.transform);
      _enemyDeath = _enemyObject.GetComponent<IEnemyDeath>();
      _enemyDeath.OnDead += HandleSpawnedDeath;
    }

    private void HandleSpawnedDeath()
    {
      _enemyDeath.OnDead -= HandleSpawnedDeath;
      _slain = true;
      _lootSpawner.SpawnLoot(_enemyObject.transform.position);
    }

    public void WriteToProgress(GameProgress playerProgress)
    {
      if (_slain)
        playerProgress.EnemiesKilled.ClearedSpawners.Add(_id);
    }
  }
}
