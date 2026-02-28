// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs.Constant
{
  /// <summary>
  /// Permanently increases the player's maximum health amount.
  /// No visual effect.
  /// Activation type: Constant.
  /// </summary>
  public class HealthBuff : BuffBase
  {
    private const string MaxHPBonusName = "MaxHealthBonus";

    private readonly PlayerHealth _playerHealth;
    private readonly float _maxHealthBonus = 25f;

    public HealthBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      IBuffDataSubservice dataSubservice,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(
        coroutineRunner,
        timeService,
        assetLoader,
        dataSubservice,
        buffStaticData,
        buffOwner
        )
    {
      _playerHealth = buffOwner.GetComponent<PlayerHealth>();
      _maxHealthBonus = dataSubservice.GetFloat(buffStaticData, MaxHPBonusName);
    }

    protected override void ConstantActivation()
    {
      _playerHealth.AddMaxHealth(_maxHealthBonus);
    }
  }
}
