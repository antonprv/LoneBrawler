// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Data.SaveData.Common;
using Code.Data.SaveData.Enemies;
using Code.Data.SaveData.Player;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Data.SaveData
{
  [Serializable]
  public sealed class GameProgress
  {
    // Player Data
    public WorldData PlayerWorldData;
    public PLayerState PLayerState;
    public PlayerStats PlayerStats;

    // Enemies Data
    public EnemiesKilled EnemiesKilled;
    public SoulsCollected SoulsCollected;

    public string CurrentScene => PlayerWorldData.TransformOnLevel.LevelName;
    public TransformData CurrentTransform => PlayerWorldData.TransformOnLevel.Transform;

    public GameProgress(IPlayerDataSubervice playerData, string initialLevel)
    {
      PlayerWorldData = new WorldData(new TransformOnLevel(initialLevel));
      PLayerState = new PLayerState(playerData);
      PlayerStats = new PlayerStats(playerData);

      EnemiesKilled = new EnemiesKilled();
      SoulsCollected = new SoulsCollected();
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
