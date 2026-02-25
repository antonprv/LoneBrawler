// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Attack;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs
{
  /// <summary>
  /// Постоянно увеличивает урон игрока на DamageMultiplier.
  /// Визуальный эффект спавнится у основания героя и остаётся навсегда.
  /// Тип активации: Constant.
  /// </summary>
  public class DamageBuff : BuffBase
  {
    // Без открытых полей для дизайнера — пока хардкод.
    private const float DamageMultiplier = 1.5f;

    private readonly PlayerAttack _playerAttack;

    public DamageBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(coroutineRunner, timeService, assetLoader, buffStaticData, buffOwner)
    {
      _playerAttack = buffOwner.GetComponent<PlayerAttack>();
    }

    protected override void ConstantActivation()
    {
      _playerAttack.Damage *= DamageMultiplier;
      SpawnEffectAsync(BuffOwnerTransform).Forget();
    }

    // При восстановлении из сохранения: урон уже в PlayerStats, только возвращаем визуал.
    protected override void OnConstantRestored()
    {
      SpawnEffectAsync(BuffOwnerTransform).Forget();
    }
  }
}
