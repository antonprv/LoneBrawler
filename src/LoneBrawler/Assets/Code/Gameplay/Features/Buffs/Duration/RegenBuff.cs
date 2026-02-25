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

namespace Code.Gameplay.Features.Buffs
{
  /// <summary>
  /// Restores the player's health over the duration of the buff.
  /// The effect fades smoothly towards the end of its action.
  /// Activation type: Duration.
  /// </summary>
  public class RegenBuff : BuffBase
  {
    // HP в секунду.
    private const float HealPerSecond = 10f;
    private const float FadeOutThreshold = 0.2f;

    private readonly PlayerHealth _playerHealth;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public RegenBuff(
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
      SpawnAndInitEffectAsync().Forget();
    }

    protected override void OnDurationTick()
    {
      _playerHealth.Heal(HealPerSecond * Time.UnscaledDeltaTime);

      if (!_fadeTriggered)
      {
        float elapsed = TotalDuration - RemainingDuration;
        float fadeStartTime = TotalDuration * (1f - FadeOutThreshold);

        if (elapsed >= fadeStartTime)
          TriggerFadeOut();
      }
    }

    protected override void OnDurationEnded()
    {
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
