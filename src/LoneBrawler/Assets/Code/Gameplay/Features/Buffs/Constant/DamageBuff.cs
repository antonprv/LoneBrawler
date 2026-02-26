// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Buffs;
using Code.Gameplay.Features.Player.Attack;
using Code.Infrastructure.AssetManagement.Interfaces;
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
    // TODO: set those through ScriptableObject
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

    // When restoring from a save: damage is already in PlayerStats, just restore visuals.
    protected override void OnConstantRestored()
    {
      SpawnEffectAsync(BuffOwnerTransform).Forget();
    }
  }
}
