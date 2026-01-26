// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;

namespace Code.Data
{
  [Serializable]
  public sealed class WorldData : IValidatableData
  {
    public TransformOnLevel TransformOnLevel;

    public WorldData(TransformOnLevel transformOnLevel)
    {
      TransformOnLevel = transformOnLevel;
    }

    public bool IsDataNull()
    {
      return TransformOnLevel.IsValid();
    }
  }
}
