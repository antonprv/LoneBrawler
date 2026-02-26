// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Gameplay.Features.Buffs;
using Code.Gameplay.Features.Player.Attack;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;
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
    private const float IncomingDamageModifier = 0.5f;  // получаем на 50% меньше урона
    private const float OutgoingDamageMultiplier = 1.5f; // наносим на 50% больше урона

    // Когда осталось меньше этой доли от длительности — начинаем фэйдаут эффекта.
    private const float FadeOutThreshold = 0.25f;

    private readonly PlayerHealth _playerHealth;
    private readonly PlayerAttack _playerAttack;

    private IParticleSmoothFade _smoothFade;
    private bool _fadeTriggered;

    public RageBuff(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      ) : base(coroutineRunner, timeService, assetLoader, buffStaticData, buffOwner)
    {
      _playerHealth = buffOwner.GetComponent<PlayerHealth>();
      _playerAttack = buffOwner.GetComponent<PlayerAttack>();
    }

    protected override void OnDurationStarted()
    {
      _playerHealth.ApplyDamageModifier(IncomingDamageModifier);
      _playerAttack.Damage *= OutgoingDamageMultiplier;

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
      _playerHealth.RemoveDamageModifier(IncomingDamageModifier);
      _playerAttack.Damage /= OutgoingDamageMultiplier;

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
