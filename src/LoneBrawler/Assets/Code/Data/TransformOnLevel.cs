// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Data
{
  [Serializable]
  public class TransformOnLevel
  {
    public TransformData Transform;
    public string LevelName;

    public TransformOnLevel(TransformData transform, string levelName)
    {
      Transform = transform;
      LevelName = levelName;
    }
  }
}
