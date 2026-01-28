// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;

namespace Code.Data
{
  [Serializable]
  public sealed class PlayerProgress
  {
    public WorldData WorldData;
    public PLayerState PLayerState;
    public PlayerStats PlayerStats;

    public string CurrentScene => WorldData.TransformOnLevel.LevelName;
    public TransformData CurrentTransform => WorldData.TransformOnLevel.Transform;

    public PlayerProgress(string initialLevel)
    {
      WorldData = new WorldData(new TransformOnLevel(initialLevel));
      PLayerState = new PLayerState();
      PlayerStats = new PlayerStats();
    }

    public bool IsWorldDataValid()
    {
      return WorldData != null
        && WorldData.IsValid();
    }

    public bool IsPlayerStatsValid()
    {
      return PlayerStats != null
        && PlayerStats.IsValid();
    }

    public bool IsPlayerDataValid()
    {
      return PLayerState != null
        && PLayerState.IsValid();
    }
  }
}
