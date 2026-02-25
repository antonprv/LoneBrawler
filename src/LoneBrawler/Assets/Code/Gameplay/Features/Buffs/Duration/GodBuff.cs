// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
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
    private const float FadeOutThreshold = 0.2f;

    private readonly PlayerHealth _playerHealth;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public GodBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(coroutineRunner, timeService, assetLoader, buffStaticData, buffOwner)
    {
      _playerHealth = buffOwner.GetComponent<PlayerHealth>();
    }

    protected override void OnDurationStarted()
    {
      _playerHealth.SetInvulnerable(true);
      SpawnAndInitEffectAsync().Forget();
    }

    protected override void OnDurationTick()
    {
      if (_fadeTriggered) return;

      float elapsed = TotalDuration - RemainingDuration;
      float fadeStartTime = TotalDuration * (1f - FadeOutThreshold);

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
      await SpawnEffectAsync(BuffOwnerTransform);

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
