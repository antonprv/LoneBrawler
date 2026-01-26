// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;

namespace Code.Data
{
  [Serializable]
  public sealed class PlayerProgress : IValidatableData
  {
    public WorldData WorldData;

    public string CurrentScene => WorldData.TransformOnLevel.LevelName;
    public TransformData CurrentTransform => WorldData.TransformOnLevel.Transform;

    public PlayerProgress(string initialLevel)
    {
      WorldData = new WorldData(new TransformOnLevel(initialLevel));
    }

    public bool IsDataNull()
    {
      return WorldData != null
        && WorldData.IsValid();
    }
  }
}
