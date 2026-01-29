// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.DataExtensions;
using Code.Data.SaveData.Common;
using Code.Data.SaveData.Enemies;
using Code.Data.SaveData.Player;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

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

    public string CurrentScene => PlayerWorldData.TransformOnLevel.LevelName;
    public TransformData CurrentTransform => PlayerWorldData.TransformOnLevel.Transform;

    public GameProgress(IStaticDataService staticDataService, string initialLevel)
    {
      staticDataService.PlayerData.Load();
      IPlayerStaticDataService playerData = staticDataService.PlayerData;

      PlayerWorldData = new WorldData(new TransformOnLevel(initialLevel));
      PLayerState = new PLayerState(playerData);
      PlayerStats = new PlayerStats(playerData);

      EnemiesKilled = new EnemiesKilled();
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
