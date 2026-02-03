// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Common;
using Code.Gameplay.Features.Enemies.Aggro.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Aggro
{
  public class Aggro : MonoBehaviour, IAggro
  {
    public TriggerObserver triggerObserver;

    public IMovableAgent _movableAgent;
    public float followDelay = 3;

    private bool _hasAggroTarget;
    private Coroutine _followCoroutine;

    public void Construct(IMovableAgent movableAgent)
    {
      _movableAgent = movableAgent;
    }

    private void Start()
    {
      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit += HandleTriggerExit;

      DontFollowPlayer();
    }

    private void OnDestroy()
    {
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
