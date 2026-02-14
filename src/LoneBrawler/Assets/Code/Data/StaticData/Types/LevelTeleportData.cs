// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes.Types;

using UnityEngine;

namespace Code.Data.StaticData.Types
{
  [System.Serializable]
  public class LevelTeleportData
  {
    public string UniqueName;
    public string LevelKey;
    public Coordinates Coords;
    public Vector3 Scale;

    public Coordinates PlayerSpawnCoords;

    public LevelTeleportData(
      string uniqueName,
      string levelKey,
      Coordinates coords,
      Vector3 scale,
      Coordinates playerSpawnCoords
      )
    {
      UniqueName = uniqueName;
      LevelKey = levelKey;
      Coords = coords;
      Scale = scale;
      PlayerSpawnCoords = playerSpawnCoords;
    }
  }
}
