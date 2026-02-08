// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions.Types;
using Code.Data.SaveData.Common;

using UnityEngine;

namespace Code.Data.StaticData.Types
{
  [Serializable]
  public class LevelTeleportData
  {
    public string LevelKey;
    public Coordinates Coords;
    public Vector3 Scale;

    public LevelTeleportData(string levelKey, Coordinates coords, Vector3 scale)
    {
      LevelKey = levelKey;
      Coords = coords;
      Scale = scale;
    }
  }
}
