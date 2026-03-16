// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;
using Code.Data.SaveData.Buffs;
using Code.Data.SaveData.Enemies;
using Code.Data.SaveData.Inventory;
using Code.Data.SaveData.Player;
using Code.Data.SaveData.Tutorials;
using Code.Data.SaveData.Types;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Data.SaveData
{
  [System.Serializable]
  public sealed class GameProgress
  {
    public long SaveTimeUTC;

    // Player Data
    public WorldData PlayerWorldData;
    public PLayerState PLayerState;
    public PlayerStats PlayerStats;

    // Enemies Data
    public EnemiesKilled EnemiesKilled;
    public SoulsCollected SoulsCollected;

    public BuffsRegistry BuffsRegistry;
    public InventorySaveData Inventory;

    public WatchedTutorials WatchedTutorials;

    public string CurrentScene => PlayerWorldData.TransformOnLevel.LevelName;
    public TransformData CurrentTransform => PlayerWorldData.TransformOnLevel.Transform;

    public GameProgress(
      IPlayerDataSubervice playerData,
      IInventoryConfigSubservice inventoryConfig,
      string initialLevel
      )
    {
      SaveTimeUTC = 0;

      PlayerWorldData = new WorldData(new TransformOnLevel(initialLevel));
      PLayerState = new PLayerState(playerData);
      PlayerStats = new PlayerStats(playerData);

      EnemiesKilled = new EnemiesKilled();
      SoulsCollected = new SoulsCollected();
      BuffsRegistry = new BuffsRegistry();

      WatchedTutorials = new WatchedTutorials();

      Inventory = new InventorySaveData();
      Inventory.InitializeSlots(
        inventoryConfig.InventorySize,
        inventoryConfig.HotbarSize);
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
