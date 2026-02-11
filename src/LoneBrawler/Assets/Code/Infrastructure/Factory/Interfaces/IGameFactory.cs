// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Threading.Tasks;

using Code.Common.Extensions.CustomTypes.Types;

using Code.Data.StaticData.Types;

using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Factory.Interfaces
{
  public interface IGameFactory
  {
    List<IProgressReader> ProgressReaders { get; }
    List<IProgressWriter> ProgressWriters { get; }

    /// <summary>
    /// Creates a hero and places it at the Vector3.zero world coordinates.
    /// </summary>
    /// <returns>GameObject</returns>
    public GameObject CreatePlayer();

    /// <summary>
    /// Creates a hero and places it at the PlayerStart object.
    /// Hero will be facing the same way the arrow in PlayerStart does.
    /// </summary>
    /// <returns>GameObject</returns>
    public GameObject CreateAndPlacePlayer(Coordinates coordinates);

    /// <summary>
    /// Creates base HUD class and adds to the scene.
    /// </summary>
    /// <returns>GameObject</returns>
    public GameObject CreateHud();

    public Task<GameObject> CreateEnemy(EnemyTypeId typeID, Transform parent);

    public Task<GameObject> CreateLoot(EnemyTypeId typeId, Vector3 position);

    public Task CreateEnemySpawnerAsync(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId);

    public Task CreateTeleportAsync(Coordinates coords, Vector3 scale, string levelKey, string uniqueName);

    void Cleanup();
  }
}
