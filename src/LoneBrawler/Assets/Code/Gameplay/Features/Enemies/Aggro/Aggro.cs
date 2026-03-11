// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Common.UtilityComponents;
using Code.Data.StaticData;
using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
using Code.Gameplay.Features.Enemies.Aggro.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Gameplay.Utils.ActorComponents;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Core;

namespace Code.Gameplay.Features.Enemies.Aggro
{
  public class Aggro : AsyncStartMonoBehaviour, IAggro, IEnemyStaticDataReceiver
  {
    public TriggerObserver triggerObserver;
    public SoundPlayer soundPlayer;

    [Zenjex] private readonly IGameLog _logger;

    public IMovableAgent _movableAgent;
    public float followDelay = 3;

    private bool _hasAggroTarget;
    private Coroutine _followCoroutine;
    private bool _isInitialized;
    private bool _shouldMove;

    public void SetValues(EnemyStaticData enemyStaticData) =>
      _shouldMove = enemyStaticData.ShouldMove;

    public void Construct(IMovableAgent movableAgent) =>
      _movableAgent = movableAgent;

    protected override void AsyncStart()
    {
      if (!_shouldMove) return;

      if (triggerObserver == null)
      {
        _logger.Log(LogType.Error,
          $"{nameof(triggerObserver)} is missing on {gameObject.name}");
        return;
      }

      PerformStart();
    }

    private void PerformStart()
    {
      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit += HandleTriggerExit;

      DontFollowPlayer();
    }

    public void Activate()
    {
      if (!_shouldMove) return;

      enabled = true;
    }

    public void Deactivate()
    {
      if (!_shouldMove) return;

      enabled = false;
    }

    private void OnDestroy()
    {
      if (!_isInitialized) return;

      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit -= HandleTriggerExit;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      if (!_shouldMove) return;

      if (!_hasAggroTarget)
      {
        _hasAggroTarget = true;

        if (_followCoroutine != null)
          StopCoroutine(_followCoroutine);

        FollowPlayer();

        soundPlayer.PlaySound(SoundType.Aggro).Forget();
      }
    }

    private void HandleTriggerExit(Collider collider)
    {
      if (!_shouldMove) return;

      if (_hasAggroTarget)
      {
        _hasAggroTarget = false;

        _followCoroutine = StartCoroutine(StopFollowingAfterDelay());
      }
    }

    private IEnumerator StopFollowingAfterDelay()
    {
      yield return new WaitForSeconds(followDelay);
      DontFollowPlayer();
    }

    private void DontFollowPlayer()
    {
      if (!_shouldMove) return;

      _movableAgent.StopFollowingImmediately();
    }

    private void FollowPlayer()
    {
      if (!_shouldMove) return;

      _movableAgent.ContinueFollowing();
    }
  }
}
