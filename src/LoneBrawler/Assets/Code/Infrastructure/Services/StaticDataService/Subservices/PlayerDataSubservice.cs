// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class PlayerDataSubservice : IPlayerDataSubervice
  {
    public float MaxHealth => _playerData.PlayerMaxHealth;
    public float AttackDamage => _playerData.PlayerAttackDamage;
    public float AttackRange => _playerData.PlayerAttackRange;
    public float AttackRadius => _playerData.PlayerAttackRadius;
    public int MaxEnemiesHit => _playerData.PlayerMaxEnemiesHit;

    private PlayerStaticData _playerData;

    private IGameLog _logger;

    public PlayerDataSubservice()
    {
      _logger = RootContext.Resolve<IGameLog>();
    }

    public void LoadSelf()
    {
      if (!_playerData)
      {
        _playerData = Resources.Load<PlayerStaticData>("StaticData/PlayerStaticData");

        if (!_playerData)
          _logger.Log
            (LogType.Warning,
            $"{typeof(PlayerStaticData)} not found!" +
            $" Make sure it's in a Resources folder with correct path");
      }
    }

  }
}
