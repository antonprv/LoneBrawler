// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Data
{
  [Serializable]
  public sealed class TransformOnLevel
  {
    public TransformData Transform;
    public string LevelName;

    public TransformOnLevel(string levelName)
    {
      Transform = TransformData.Identity();
      LevelName = levelName;
    }

    public TransformOnLevel(TransformData transform, string levelName)
    {
      Transform = transform;
      LevelName = levelName;
    }
  }
}
