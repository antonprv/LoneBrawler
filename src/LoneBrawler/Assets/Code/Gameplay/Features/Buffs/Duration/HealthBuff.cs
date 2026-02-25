// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Time;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs
{
  /// <summary>
  /// Навсегда увеличивает максимальное количество здоровья игрока.
  /// Визуального эффекта нет.
  /// Тип активации: Constant.
  /// </summary>
  public class HealthBuff : BuffBase
  {
    private const float MaxHealthBonus = 25f;

    private readonly PlayerHealth _playerHealth;

    public HealthBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(coroutineRunner, timeService, assetLoader, buffStaticData, buffOwner)
    {
      _playerHealth = buffOwner.GetComponent<PlayerHealth>();
    }

    protected override void ConstantActivation()
    {
      _playerHealth.AddMaxHealth(MaxHealthBonus);
    }
  }
}
