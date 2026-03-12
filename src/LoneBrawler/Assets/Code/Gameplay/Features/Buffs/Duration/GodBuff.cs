// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs.Duration
{
  /// <summary>
  /// Makes the player completely immune to all damage for the duration.
  /// The effect fades smoothly towards the end of the buff.
  /// Activation type: Duration.
  /// </summary>
  public class GodBuff : BuffBase
  {
    private const string FadeOutThresholdName = "FadeOutThreshold";

    private readonly PlayerHealth _playerHealth;
    private readonly float _fadeOutThreshold = 0.2f;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public GodBuff(
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
      dataSubservice.GetFloat(buffStaticData, FadeOutThresholdName);
    }

    protected override void OnDurationStarted()
    {
      _fadeTriggered = false;
      _playerHealth.SetInvulnerable(true);
      SpawnAndInitEffectAsync().Forget();
    }

    protected override void OnDurationTick()
    {
      if (_fadeTriggered) return;

      float elapsed = TotalDuration - RemainingDuration;
      float fadeStartTime = TotalDuration * (1f - _fadeOutThreshold);

      if (elapsed >= fadeStartTime)
        TriggerFadeOut();
    }

    protected override void OnDurationEnded()
    {
      _playerHealth.SetInvulnerable(false);
      TriggerFadeOut();
    }

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
