// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Data.StaticData.Types;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Spawn
{
  public class EnemySpawnPoint : MonoBehaviour, IProgressReader, IProgressWriter
  {
    public string Id { get; private set; }

    private bool _slain;

    private IGameFactory _gameFactory;
    private ILootSpawner _lootSpawner;
    private EnemyTypeId _enemyTypeId;
    private GameObject _enemyObject;
    private IEnemyDeath _enemyDeath;
    private bool _isSpawned;

    public void Construct(
      IGameFactory gameFactory,
      string spawnerId,
      EnemyTypeId enemyTypeId,
      ILootSpawner lootSpawner)
    {
      Id = spawnerId;
      _gameFactory = gameFactory;
      _lootSpawner = lootSpawner;
      _enemyTypeId = enemyTypeId;
    }

    public void ReadProgress(GameProgress playerProgress)
    {
      if (playerProgress.EnemiesKilled.ClearedSpawners.Contains(Id))
        _slain = true;
      else
      {
        Spawn();
      }
    }

    private void Spawn()
    {
      if (_isSpawned) return;

      _enemyObject = _gameFactory.CreateEnemy(_enemyTypeId, gameObject.transform);
      _enemyDeath = _enemyObject.GetComponent<IEnemyDeath>();
      _enemyDeath.OnDead += HandleSpawnedDeath;

      _isSpawned = true;
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
        playerProgress.EnemiesKilled.ClearedSpawners.Add(Id);
    }
  }
}
