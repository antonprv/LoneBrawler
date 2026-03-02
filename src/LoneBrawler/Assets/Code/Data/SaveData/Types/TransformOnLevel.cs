// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.VectorTypes.Interfaces;
using Code.Common.Domain.DataTypes;

namespace Code.Data.SaveData.Types
{
  [System.Serializable]
  public sealed class TransformOnLevel : IValidatableData
  {
    public TransformData Transform;
    public string LevelName;

    private readonly bool _initialSave;

    public TransformOnLevel(string levelName)
    {
      LevelName = levelName;
      _initialSave = true;
    }

    public TransformOnLevel(TransformData transform, string levelName)
    {
      Transform = transform;
      LevelName = levelName;
      _initialSave = false;
    }

    public bool IsValid()
    {
      return !_initialSave
        && !string.IsNullOrWhiteSpace(LevelName)
        && !string.IsNullOrEmpty(LevelName);
    }
  }
}
