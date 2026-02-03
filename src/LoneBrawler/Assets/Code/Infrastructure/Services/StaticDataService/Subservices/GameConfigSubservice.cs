// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData.Configs;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class GameConfigSubservice : IGameConfigSubservice
  {
    // Gameplay Tags
    public string PlayerStartTag => _gameconfig.PlayerStartTag;

    // Physics Layers
    public int PlayerCollision => 1 << _gameconfig.PlayerLayer;
    public int EnemyHitableLayer => 1 << _gameconfig.EnemyHitableLayer;
    public int LootLayer => 1 << _gameconfig.LootLayer;

    private IGameLog _logger;

    private GameConfig _gameconfig;

    public GameConfigSubservice()
    {
      _logger = RootContext.Resolve<IGameLog>();
      LoadSelf();
    }

    private void LoadSelf()
    {
      if (!_gameconfig)
      {
        _gameconfig = Resources.Load<GameConfig>("StaticData/Config/GameConfig");

        if (!_gameconfig)
          _logger.Log
            (LogType.Warning,
            $"{typeof(GameConfig)} not found!" +
            $" Make sure it's in a Resources folder with correct path");
      }
    }
  }
}
