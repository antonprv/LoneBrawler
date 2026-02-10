// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace Code.Data.DataExtensions.Types
{
  [Serializable]
  public class Coordinates
  {
    public Vector3 Position;
    public Quaternion Rotation;

    public Coordinates(Vector3 location, Quaternion rotation)
    {
      Position = location;
      Rotation = rotation;
    }

    public static Coordinates Identity() =>
      new Coordinates(Vector3.one, Quaternion.identity);
  }
}
