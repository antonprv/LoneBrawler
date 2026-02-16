// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Common.CustomTypes.Infrastructure.Types;

using Code.Data.StaticData.Types;

namespace Code.Data.StaticData
{
  [UnityEngine.CreateAssetMenu(fileName = "LevelStaticData",
    menuName = "StaticData/LevelStaticData")]
  public class LevelStaticData : UnityEngine.ScriptableObject
  {
    public string LevelKey;

    public List<EnemySpawnerData> EnemySpawners = new List<EnemySpawnerData>();

    public List<LevelTeleportData> Teleports = new List<LevelTeleportData>();

    public Coordinates PlayerStartCoordinates = Coordinates.Identity();
  }
}
