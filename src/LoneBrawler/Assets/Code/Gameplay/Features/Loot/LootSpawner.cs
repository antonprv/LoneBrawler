// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Data.SaveData;
using Code.Data.StaticData.Types.Enemies;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Loot
{
  public class LootSpawner : ZenjexBehaviour, ILootSpawner, IProgressReader, IProgressWriter
  {
    public Vector3 spawnOffset;

    [Zenjex] private readonly IGameFactory _gameFactory;

    private EnemyTypeId _typeId;
    private string _enemyId;
    private string _id;
    private bool _collected;
    private bool _lootSpawned;
    private ILoot _loot;
    private Vector3 _spawnedPosition;

    private readonly CompositeDisposable _disposables = new();

    public void Construct(
      string spawnerId,
      EnemyTypeId enemyTypeId
      )
    {
      _typeId = enemyTypeId;
      _enemyId = spawnerId;
      _id = $"Loot_{spawnerId}";
    }

    private void OnDestroy() => _disposables.Dispose();

    public async UniTaskVoid SpawnLoot(Vector3 position)
    {
      if (_lootSpawned) return;

      CancellationToken ct = this.GetCancellationTokenOnDestroy();

      _spawnedPosition = position == Vector3.zero
        ? gameObject.transform.position + spawnOffset
        : position + spawnOffset;

      GameObject createdLoot = await
        _gameFactory.CreateLoot(_typeId, _spawnedPosition);

      if (ct.IsCancellationRequested) return;

      _loot = createdLoot.GetComponent<ILoot>();
      _loot.OnCollected
        .Skip(1)
        .Subscribe(_ => HandleCollected())
        .AddTo(_disposables);

      _lootSpawned = true;
    }

    private void HandleCollected() => _collected = true;

    public void ReadProgress(GameProgress playerProgress)
    {
      if (IsEnemyKilled(playerProgress) && IsLootLeft(playerProgress))
        SpawnLoot(playerProgress.SoulsCollected.LeftSpawners[_id]).Forget();
    }

    private bool IsLootLeft(GameProgress playerProgress) =>
      playerProgress.SoulsCollected.LeftSpawners.ContainsKey(_id);

    private bool IsEnemyKilled(GameProgress playerProgress) =>
      playerProgress.EnemiesKilled.ClearedSpawners.Contains(_enemyId);

    public void WriteToProgress(GameProgress playerProgress)
    {
      if (_lootSpawned && !_collected)
        playerProgress.SoulsCollected.LeftSpawners.TryAdd(_id, _spawnedPosition);
      else if (_collected && playerProgress.SoulsCollected.LeftSpawners.ContainsKey(_id))
        playerProgress.SoulsCollected.LeftSpawners.Remove(_id);
    }
  }
}
