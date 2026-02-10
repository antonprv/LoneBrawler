// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Data.DataExtensions.Interfaces;

namespace Code.Data.SaveData.Common
{
  [Serializable]
  public sealed class WorldData : IValidatableData
  {
    public TransformOnLevel TransformOnLevel;

    public string LastTeleportUniqueName;

    public long LastTeleportTimeUTC;

    public WorldData(TransformOnLevel transformOnLevel)
    {
      TransformOnLevel = transformOnLevel;
      LastTeleportUniqueName = null;
      LastTeleportTimeUTC = 0;
    }

    public bool IsDataNull() => TransformOnLevel.IsValid();
  }
}
