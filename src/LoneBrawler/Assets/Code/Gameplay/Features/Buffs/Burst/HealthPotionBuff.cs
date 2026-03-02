// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs.Burst
{
  /// <summary>
  /// Burst-buff: instantly restores a fixed amount of health,
  /// then immediately disappears. The effect is reproduced briefly and then removed.
  /// Activation type: Burst.
  /// </summary>
  public class HealthPotionBuff : BuffBase
  {
    private const string HealAmountName = "HealAmount";
    private const string EffectLifetimeName = "EffectLifetime";

    private readonly PlayerHealth _playerHealth;

    private readonly float _healAmount;
    private readonly float _effectLifetime; // seconds before effect gets deleted

    public HealthPotionBuff(
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

      _healAmount = dataSubservice.GetFloat(buffStaticData, HealAmountName);
      _effectLifetime = dataSubservice.GetFloat(buffStaticData, EffectLifetimeName);
    }

    protected override void BurstActivation()
    {
      _playerHealth.Heal(_healAmount);
      // Спавним эффект и запускаем его плавное угасание через ParticleSmoothFade.
      SpawnAndFadeEffectAsync().Forget();
    }

    private async UniTaskVoid SpawnAndFadeEffectAsync()
    {
      await SpawnEffectAsync(BuffOwnerTransform);

      if (SpawnedEffect == null) return;

      var smoothFade = SpawnedEffect.GetComponentInChildren<IParticleSmoothFade>();

      if (smoothFade != null)
      {
        smoothFade.OnStopped += DestroyEffect;
        smoothFade.TriggerStop();
      }
      else
      {
        // Если на префабе нет ParticleSmoothFade — просто удаляем через задержку.
        GameObject.Destroy(SpawnedEffect, _effectLifetime);
      }
    }
  }
}
