// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;
using System.Threading.Tasks;

using Code.Data.SaveData;
using Code.Data.StaticData.Types.Enemies;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Enemies.Spawn
{
  public class EnemySpawnPoint : ZenjexBehaviour, IProgressReaderAsync, IProgressWriter
  {
    public string Id { get; private set; }

    [Zenjex] private readonly IGameFactory _gameFactory;

    private bool _slain;

    private ILootSpawner _lootSpawner;
    private EnemyTypeId _enemyTypeId;
    private GameObject _enemyObject;
    private IEnemyDeath _enemyDeath;
    private bool _isSpawned;

    private CompositeDisposable _disposables = new();

    public void Construct(
      string spawnerId,
      EnemyTypeId enemyTypeId,
      ILootSpawner lootSpawner)
    {
      Id = spawnerId;
      _lootSpawner = lootSpawner;
      _enemyTypeId = enemyTypeId;
    }

    private void OnDestroy() => _disposables.Dispose();

    public async UniTask ReadProgressAsync(GameProgress playerProgress)
    {
      if (playerProgress.EnemiesKilled.ClearedSpawners.Contains(Id))
      {
        _slain = true;
        return;
      }

      await Spawn();
    }

    private async UniTask Spawn()
    {
      if (_isSpawned) return;

      CancellationToken ct = this.GetCancellationTokenOnDestroy();

      _enemyObject = await _gameFactory.CreateEnemy(_enemyTypeId, gameObject.transform);
      _enemyObject.transform.rotation = transform.rotation;
      _enemyDeath = _enemyObject.GetComponent<IEnemyDeath>();

      if (ct.IsCancellationRequested) return;

      SubscribeToRP();

      _isSpawned = true;
    }

    private void SubscribeToRP() => _enemyDeath.OnDead
        .Take(1)
        .Subscribe(_ => HandleSpawnedDeath())
        .AddTo(_disposables);

    private void HandleSpawnedDeath()
    {
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
