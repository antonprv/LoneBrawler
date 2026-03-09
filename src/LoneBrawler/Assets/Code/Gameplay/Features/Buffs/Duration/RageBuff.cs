// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Attack;
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
  /// Reduces incoming damage and increases outgoing damage during the buff period.
  /// Effect appears gradually and fades out smoothly at the end.
  /// Activation type: Duration.
  /// </summary>
  public class RageBuff : BuffBase
  {
    private const string IncomingDamageModifierName = "IncomingDamageModifier";
    private const string OutgoingDamageMultiplierName = "OutgoingDamageMultiplier";
    private const string FadeOutThresholdName = "FadeOutThreshold";

    private readonly PlayerHealth _playerHealth;
    private readonly PlayerAttack _playerAttack;

    private readonly float _incomingDamageModifier;  // receive % less damage
    private readonly float _outgoingDamageMultiplier; // deal % more damage
    private readonly float _fadeOutThreshold;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public RageBuff(
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
      _playerAttack = buffOwner.GetComponent<PlayerAttack>();

      _incomingDamageModifier = dataSubservice.GetFloat(buffStaticData, IncomingDamageModifierName);
      _outgoingDamageMultiplier = dataSubservice.GetFloat(buffStaticData, OutgoingDamageMultiplierName);
      _fadeOutThreshold = dataSubservice.GetFloat(buffStaticData, FadeOutThresholdName);
    }

    protected override void OnDurationStarted()
    {
      _fadeTriggered = false;

      _playerHealth.ApplyDamageModifier(_incomingDamageModifier);
      _playerAttack.Damage *= _outgoingDamageMultiplier;

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
      _playerHealth.RemoveDamageModifier(_incomingDamageModifier);
      _playerAttack.Damage /= _outgoingDamageMultiplier;

      // If fade hasn't been started yet (for example, if the buff ended abruptly), start it now.
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
