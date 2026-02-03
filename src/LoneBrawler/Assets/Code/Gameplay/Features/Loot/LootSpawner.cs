// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.SaveData;
using Code.Data.StaticData.Types;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Gameplay.Features.Loot.TrackerService.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
  public class LootSpawner : MonoBehaviour, ILootSpawner, IProgressReader, IProgressWriter
  {
    public Vector3 spawnOffset;

    private IGameFactory _gameFactory;

    private EnemyTypeId _typeId;
    private string _enemyId;
    private string _id;
    private bool _collected;
    private ILoot _loot;
    private ILootTrackerService _lootTracker;
    private Vector3 _spawnedPosition;

    private void Awake() =>
      _lootTracker = RootContext.Resolve<ILootTrackerService>();

    public void Construct(
      IGameFactory gameFactory,
      EnemyTypeId typeId,
      string id
      )
    {
      _gameFactory = gameFactory;
      _typeId = typeId;
      _enemyId = id;
      _id = $"Loot_{id}";
    }

    public void SpawnLoot(Vector3 position)
    {
      _spawnedPosition = position == Vector3.zero
        ? gameObject.transform.position + spawnOffset
        : position + spawnOffset;

      GameObject createdLoot =
        _gameFactory.CreateLoot(_typeId, _spawnedPosition);

      _loot = createdLoot.GetComponent<ILoot>();
      _loot.OnCollected += HandleCollected;
    }

    private void HandleCollected()
    {
      _loot.OnCollected -= HandleCollected;
      _collected = true;
      GiveSoulsToPlayer();
    }

    private void GiveSoulsToPlayer() => _lootTracker.Souls += _loot.Souls;

    public void ReadProgress(GameProgress playerProgress)
    {
      if (IsEnemyKilled(playerProgress) && IsLootLeft(playerProgress))
        SpawnLoot(playerProgress.SoulsCollected.LeftSpawners[_id]);
    }

    private bool IsLootLeft(GameProgress playerProgress) =>
      playerProgress.SoulsCollected.LeftSpawners.ContainsKey(_id);

    private bool IsEnemyKilled(GameProgress playerProgress) =>
      playerProgress.EnemiesKilled.ClearedSpawners.Contains(_enemyId);

    public void WriteToProgress(GameProgress playerProgress)
    {
      if (!_collected)
        playerProgress.SoulsCollected.LeftSpawners.TryAdd(_id, _spawnedPosition);
    }
  }
}
