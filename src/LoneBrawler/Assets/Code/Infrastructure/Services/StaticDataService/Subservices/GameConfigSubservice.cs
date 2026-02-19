// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Configs;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class GameConfigSubservice : IGameConfigSubservice
  {
    // Gameplay Tags
    public string PlayerTag => _gameconfig.PlayerTag;
    public string PlayerStartTag => _gameconfig.PlayerStartTag;
    public string EnemyTag => _gameconfig.EnemyTag;
    public string EnemySpawnerTag => _gameconfig.EnemySpawnerTag;

    // Physics Layers
    public int PlayerLayerBitmask => 1 << _gameconfig.PlayerLayer;
    public int EnemyHitableLayerBitmask => 1 << _gameconfig.EnemyHitableLayer;
    public int LootLayerBitmask => 1 << _gameconfig.LootLayer;
    public int AggroLayerBitmask => 1 << _gameconfig.AggroLayer;
    public int AttackZoneLayerBitmask => 1 << _gameconfig.AttackZoneLayer;
    public int SaveTriggerLayerBitmask => 1 << _gameconfig.SaveTriggerLayer;

    public int PlayerLayer => _gameconfig.PlayerLayer;
    public int EnemyHitableLayer => _gameconfig.EnemyHitableLayer;
    public int LootLayer => _gameconfig.LootLayer;
    public int AggroLayer => _gameconfig.AggroLayer;
    public int AttackZoneLayer => _gameconfig.AttackZoneLayer;
    public int SaveTriggerLayer => _gameconfig.SaveTriggerLayer;


    private IGameLog _logger;
    private IAssetLoader _assetLoader;
    private GameConfig _gameconfig;

    public GameConfigSubservice(IGameLog gameLog, IAssetLoader assetLoader)
    {
      _logger = gameLog;
      _assetLoader = assetLoader;
    }

    public async Task LoadSelfAsync()
    {
      if (!_gameconfig)
      {
        _gameconfig = await _assetLoader.LoadAsync<GameConfig>(StaticDataAddresses.GameConfigAddress);

        if (!_gameconfig)
          _logger.Log
            (LogType.Warning,
            $"{typeof(GameConfig)} not found!" +
            $" Make sure it's in a Resources folder with correct path");
      }
    }
  }
}
