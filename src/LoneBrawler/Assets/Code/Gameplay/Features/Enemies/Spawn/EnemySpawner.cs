// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.SaveData;
using Code.Data.StaticData;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Spawn
{
  public class EnemySpawner : MonoBehaviour, IProgressReader, IProgressWriter
  {
    public EnemyTypeId enemyTypeId;

    private bool _slain;

    private IGameLog _logging;
    private IGameFactory _gameFactory;

    private string _id;
    private GameObject _enemyObject;
    private IEnemyDeath _enemyDeath;

    private void Awake()
    {
      _logging = RootContext.Resolve<IGameLog>();
      _gameFactory = RootContext.Resolve<IGameFactory>();

      _id = GetComponent<UniqueId>().id;
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
      _slain = true;
      _enemyDeath.OnDead -= HandleSpawnedDeath;
    }

    public void WriteToProgress(GameProgress playerProgress)
    {
      if (_slain)
        playerProgress.EnemiesKilled.ClearedSpawners.Add(_id);
    }
  }
}
