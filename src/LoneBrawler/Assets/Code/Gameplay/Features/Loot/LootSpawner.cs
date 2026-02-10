// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.SaveData;
using Code.Data.StaticData.Types;
using Code.Gameplay.Features.Loot.Interfaces;
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
    private bool _lootSpawned;
    private ILoot _loot;
    private Vector3 _spawnedPosition;

    public void Construct(
      IGameFactory gameFactory,
      string spawnerId,
      EnemyTypeId enemyTypeId
      )
    {
      _gameFactory = gameFactory;
      _typeId = enemyTypeId;
      _enemyId = spawnerId;
      _id = $"Loot_{spawnerId}";
    }

    public async void SpawnLoot(Vector3 position)
    {
      if (_lootSpawned) return;

      _spawnedPosition = position == Vector3.zero
        ? gameObject.transform.position + spawnOffset
        : position + spawnOffset;

      GameObject createdLoot = await
        _gameFactory.CreateLoot(_typeId, _spawnedPosition);

      _loot = createdLoot.GetComponent<ILoot>();
      _loot.OnCollected += HandleCollected;

      _lootSpawned = true;
    }

    private void HandleCollected()
    {
      _loot.OnCollected -= HandleCollected;
      _collected = true;
    }

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
      if (_lootSpawned && !_collected)
        playerProgress.SoulsCollected.LeftSpawners.TryAdd(_id, _spawnedPosition);
      else if (_collected && playerProgress.SoulsCollected.LeftSpawners.ContainsKey(_id))
        playerProgress.SoulsCollected.LeftSpawners.Remove(_id);
    }
  }
}
