// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.VectorTypes.Interfaces;
using Code.Common.Domain.DataTypes;

namespace Code.Data.SaveData.Types
{
  [System.Serializable]
  public sealed class TransformOnLevel : IValidatableData
  {
    public TransformData Transform = null;
    public string LevelName;

    public TransformOnLevel(string levelName) => LevelName = levelName;

    public TransformOnLevel(TransformData transform, string levelName)
    {
      Transform = transform;
      LevelName = levelName;
    }

    public bool IsValid()
    {
      return Transform != null
        && !string.IsNullOrWhiteSpace(LevelName)
        && !string.IsNullOrEmpty(LevelName);
    }
  }
}
