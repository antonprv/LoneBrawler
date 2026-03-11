// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs.Duration
{
  /// <summary>
  /// Restores the player's health over the duration of the buff.
  /// The effect fades smoothly towards the end of its action.
  /// Activation type: Duration.
  /// </summary>
  public class RegenBuff : BuffBase
  {
    private const string HealPerSecondName = "HealPerSecond";
    private const string FadeOutThresholdName = "FadeOutThreshold";

    private readonly PlayerHealth _playerHealth;

    private readonly float _healPerSecond;
    private readonly float _fadeOutThreshold;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public RegenBuff(
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

      _healPerSecond = dataSubservice.GetFloat(buffStaticData, HealPerSecondName);
      _fadeOutThreshold = dataSubservice.GetFloat(buffStaticData, FadeOutThresholdName);
    }

    protected override void OnDurationStarted()
    {
      _fadeTriggered = false;
      SpawnAndInitEffectAsync().Forget();
    }

    protected override void OnDurationTick()
    {
      _playerHealth.Heal(_healPerSecond * Time.UnscaledDeltaTime);

      if (!_fadeTriggered)
      {
        float elapsed = TotalDuration - RemainingDuration;
        float fadeStartTime = TotalDuration * (1f - _fadeOutThreshold);

        if (elapsed >= fadeStartTime)
          TriggerFadeOut();
      }
    }

    protected override void OnDurationEnded() => TriggerFadeOut();

    private async UniTaskVoid SpawnAndInitEffectAsync()
    {
      CancellationToken ct = BuffOwner.GetCancellationTokenOnDestroy();

      await SpawnEffectAsync(BuffOwnerTransform, ct);

      if (SpawnedEffect == null) return;

      _smoothFade = SpawnedEffect.GetComponentInChildren<IParticleSmoothFade>();
    }

    private void TriggerFadeOut()
    {
      if (_fadeTriggered) return;
      _fadeTriggered = true;

      _smoothFade?.TriggerStop();
    }
  }
}
