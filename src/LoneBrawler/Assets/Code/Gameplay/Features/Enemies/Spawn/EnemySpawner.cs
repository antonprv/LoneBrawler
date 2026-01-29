// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.SaveData;
using Code.Data.StaticData;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Spawn
{
  public class EnemySpawner : MonoBehaviour, IProgressReader, IProgressWriter
  {
    public EnemyTypeId enemyTypeId;

    public bool slain;

    private IGameLog _logging;

    private string _id;

    private void Awake()
    {
      _logging = RootContext.Resolve<IGameLog>();

      _id = GetComponent<UniqueId>().id;
    }

    public void ReadProgress(GameProgress playerProgress)
    {
      if (playerProgress.EnemiesKilled.ClearedSpawners.Contains(_id))
        slain = true;
      else
      {
        Spawn();
      }
    }

    private void Spawn()
    {
      _logging.Log($"Spawner {_id}: spawned an enemy!");
    }

    public void WriteToProgress(GameProgress playerProgress)
    {
      if (slain)
        playerProgress.EnemiesKilled.ClearedSpawners.Add(_id);
    }
  }
}
