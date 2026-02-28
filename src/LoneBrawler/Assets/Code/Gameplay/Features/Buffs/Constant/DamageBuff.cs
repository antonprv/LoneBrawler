// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Attack;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs.Constant
{
  /// <summary>
  /// Permanently increases the player's damage by DamageMultiplier.
  /// Visual effect is spawned at the hero's base and remains forever.
  /// Activation type: Constant.
  /// </summary>
  public class DamageBuff : BuffBase
  {
    private const string DamageMulName = "DamageMultiplier";

    private readonly PlayerAttack _playerAttack;
    private readonly float _damageMultiplier;

    public DamageBuff(
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
      _playerAttack = buffOwner.GetComponent<PlayerAttack>();
      _damageMultiplier = dataSubservice.GetFloat(buffStaticData, DamageMulName);
    }

    protected override void ConstantActivation()
    {
      _playerAttack.Damage *= _damageMultiplier;
      SpawnEffectAsync(BuffOwnerTransform).Forget();
    }

    // When restoring from a save: damage is already in PlayerStats, just restore visuals.
    protected override void OnConstantRestored()
    {
      SpawnEffectAsync(BuffOwnerTransform).Forget();
    }
  }
}
