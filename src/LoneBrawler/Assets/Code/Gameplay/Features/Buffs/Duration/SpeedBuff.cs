// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Buffs;
using Code.Gameplay.Features.Player.Movement;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs.Duration
{
  /// <summary>
  /// Increases the player's movement speed during the buff period.
  /// The effect fades smoothly towards the end of the buff.
  /// Activation type: Duration.
  /// </summary>
  public class SpeedBuff : BuffBase
  {
    private const float SpeedMultiplier = 1.75f;
    private const float FadeOutThreshold = 0.2f;

    private readonly PlayerMove _playerMove;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public SpeedBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(coroutineRunner, timeService, assetLoader, buffStaticData, buffOwner)
    {
      _playerMove = buffOwner.GetComponent<PlayerMove>();
    }

    protected override void OnDurationStarted()
    {
      _playerMove.ApplySpeedMultiplier(SpeedMultiplier);
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
      _playerMove.RemoveSpeedMultiplier(SpeedMultiplier);
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
