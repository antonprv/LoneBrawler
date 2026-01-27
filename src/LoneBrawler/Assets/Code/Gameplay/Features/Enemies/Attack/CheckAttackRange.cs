// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(Attack))]
  public class CheckAttackRange : MonoBehaviour
  {
    public Attack attack;
    public TriggerObserver triggerObserver;

    private void Awake()
    {
      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit += HandleTriggerExit;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      attack.EnableAttack();
    }

    private void HandleTriggerExit(Collider collider)
    {
      attack.DisableAttack();
    }

    private void OnDestroy()
    {
      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit -= HandleTriggerExit;
    }

  }
}
