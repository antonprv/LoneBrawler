// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Common;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies
{
  public class Aggro : MonoBehaviour
  {
    public TriggerObserver triggerObserver;

    public IMovableAgent _movableAgent;
    public float followDelay = 3;

    private bool _hasAggroTarget;
    private Coroutine _followCoroutine;

    private void Awake()
    {
      _movableAgent = gameObject.GetComponentInChildren<IMovableAgent>();
    }

    private void Start()
    {
      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit += HandleTriggerExit;

      DontFollowPlayer();
    }

    private void OnDisable()
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
      _movableAgent.StopMovingImmediately();
      _movableAgent.DisableSelf();
    }

    private void FollowPlayer()
    {
      _movableAgent.ContinueFollowing();
      _movableAgent.EnableSelf();
    }
  }
}
