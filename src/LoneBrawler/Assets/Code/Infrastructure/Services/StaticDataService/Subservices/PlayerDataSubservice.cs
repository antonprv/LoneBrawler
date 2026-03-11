// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class PlayerDataSubservice : IPlayerDataSubervice
  {
    // Health
    public float MaxHealth => _playerData.PlayerMaxHealth;

    // Attack
    public float AttackDamage => _playerData.PlayerAttackDamage;
    public float AttackRange => _playerData.PlayerAttackRange;
    public float AttackRadius => _playerData.PlayerAttackRadius;
    public int MaxEnemiesHit => _playerData.PlayerMaxEnemiesHit;

    // Movement
    public float MovementSpeed => _playerData.MovementSpeed;
    public float RotationSpeed => _playerData.RotationSpeed;

    // Death
    public float DeathDelay => _playerData.DeathDelay;

    private PlayerStaticData _playerData;

    private readonly IGameLog _logger;
    private readonly IAssetLoader _assetLoader;

    public PlayerDataSubservice(IGameLog gameLog, IAssetLoader assetLoader)
    {
      _logger = gameLog;
      _assetLoader = assetLoader;
    }

    public async UniTask LoadSelfAsync()
    {
      if (!_playerData)
      {
        _playerData = await _assetLoader.LoadAsync<PlayerStaticData>(StaticDataAddresses.PlayerDataAddress);

        if (!_playerData)
          _logger.Log
            (LogType.Warning,
            $"{typeof(PlayerStaticData)} not found!" +
            $" Make sure it's in a Resources folder with correct path");
      }
    }

  }
}
