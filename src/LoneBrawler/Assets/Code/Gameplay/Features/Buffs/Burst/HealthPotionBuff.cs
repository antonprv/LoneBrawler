// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs
{
  /// <summary>
  /// Burst-бафф: мгновенно восстанавливает фиксированное количество здоровья,
  /// затем немедленно исчезает. Эффект воспроизводится ненадолго и потом удаляется.
  /// Тип активации: Burst.
  /// </summary>
  public class HealthPotionBuff : BuffBase
  {
    private const float HealAmount = 50f;
    private const float EffectLifetime = 3f; // секунды до удаления эффекта

    private readonly PlayerHealth _playerHealth;

    public HealthPotionBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(coroutineRunner, timeService, assetLoader, buffStaticData, buffOwner)
    {
      _playerHealth = buffOwner.GetComponent<PlayerHealth>();
    }

    protected override void BurstActivation()
    {
      _playerHealth.Heal(HealAmount);

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
        GameObject.Destroy(SpawnedEffect, EffectLifetime);
      }
    }
  }
}
