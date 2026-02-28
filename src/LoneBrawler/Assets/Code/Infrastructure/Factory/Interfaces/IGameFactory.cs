// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Common.CustomTypes.Infrastructure.Types;
using Code.Data.StaticData.Types.Enemies;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Factory.Interfaces
{
  public interface IGameFactory
  {
    List<IProgressReader> ProgressReaders { get; }
    List<IProgressWriter> ProgressWriters { get; }

    public UniTask WarmUp();

    /// <summary>
    /// Creates a hero and places it at the Vector3.zero world coordinates.
    /// </summary>
    /// <returns>GameObject</returns>
    public UniTask<GameObject> CreatePlayerAsync();

    /// <summary>
    /// Creates a hero and places it at the PlayerStart object.
    /// Hero will be facing the same way the arrow in PlayerStart does.
    /// </summary>
    /// <returns>GameObject</returns>
    public UniTask<GameObject> CreateAndPlacePlayerAsync(Coordinates at);

    /// <summary>
    /// Creates base HUD class and adds to the scene.
    /// </summary>
    /// <returns>GameObject</returns>
    public UniTask<GameObject> CreateHudAsync();

    public UniTask<GameObject> CreateEnemy(EnemyTypeId typeID, Transform parent);

    public UniTask<GameObject> CreateLoot(EnemyTypeId typeId, Vector3 position);

    public void CreateEnemySpawner(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId);

    public void CreateTeleport(Coordinates coords, Vector3 scale, string levelKey, string uniqueName);

    public void Cleanup();
  }
}
