// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Common.UtilityComponents;
using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
using Code.Gameplay.Features.Enemies.Aggro.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Gameplay.Utils.ActorComponents;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Gameplay.Features.Enemies.Aggro
{
  public class Aggro : AsyncStartMonoBehaviour, IAggro
  {
    public TriggerObserver triggerObserver;
    public SoundPlayer soundPlayer;

    private IGameLog _logger;
    public IMovableAgent _movableAgent;
    public float followDelay = 3;

    private bool _hasAggroTarget;
    private Coroutine _followCoroutine;
    private bool _isInitialized;

    public void Construct(IMovableAgent movableAgent)
    {
      _logger = RootContext.Resolve<IGameLog>();

      _movableAgent = movableAgent;
    }

    protected override void AsyncStart()
    {
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

    public void Activate() => enabled = true;
    public void Deactivate() => enabled = false;

    private void OnDestroy()
    {
      if (!_isInitialized) return;

      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit -= HandleTriggerExit;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      if (!_hasAggroTarget)
      {
        _hasAggroTarget = true;

        if (_followCoroutine != null)
          StopCoroutine(_followCoroutine);

        FollowPlayer();

        soundPlayer.PlaySound(SoundType.Aggro);
      }
    }

    private void HandleTriggerExit(Collider collider)
    {
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
      _movableAgent.StopFollowingImmediately();
    }

    private void FollowPlayer()
    {
      _movableAgent.ContinueFollowing();
    }
  }
}
