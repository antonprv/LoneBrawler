// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.VectorTypes.Interfaces;

namespace Code.Data.SaveData.Types
{
  [System.Serializable]
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

    public bool IsValid()
    {
      return TransformOnLevel != null
        && TransformOnLevel.IsValid();
    }
  }
}
