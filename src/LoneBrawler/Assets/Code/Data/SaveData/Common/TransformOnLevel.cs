// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Data.DataExtensions.Interfaces;

namespace Code.Data.SaveData.Common
{
  [Serializable]
  public sealed class TransformOnLevel : IValidatableData
  {
    public TransformData Transform;
    public string LevelName;

    public TransformOnLevel(string levelName)
    {
      LevelName = levelName;
    }

    public TransformOnLevel(TransformData transform, string levelName)
    {
      Transform = transform;
      LevelName = levelName;
    }

    public bool IsDataNull()
    {
      return Transform != null
        && Transform.IsValid()
        && !string.IsNullOrWhiteSpace(LevelName)
        && !string.IsNullOrEmpty(LevelName);
    }
  }
}
