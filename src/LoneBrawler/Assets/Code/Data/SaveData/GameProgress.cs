// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Data.SaveData.Common;
using Code.Data.SaveData.Enemies;
using Code.Data.SaveData.Player;

namespace Code.Data.SaveData
{
  [Serializable]
  public sealed class GameProgress
  {
    public WorldData PlayerWorldData;
    public PLayerState PLayerState;
    public PlayerStats PlayerStats;

    public EnemyState[] EnemiesState;

    public string CurrentScene => PlayerWorldData.TransformOnLevel.LevelName;
    public TransformData CurrentTransform => PlayerWorldData.TransformOnLevel.Transform;

    public GameProgress(string initialLevel)
    {
      PlayerWorldData = new WorldData(new TransformOnLevel(initialLevel));
      PLayerState = new PLayerState();
      PlayerStats = new PlayerStats();
      EnemiesState = new EnemyState[0];
    }

    public bool IsWorldDataValid()
    {
      return PlayerWorldData != null
        && PlayerWorldData.IsValid();
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
