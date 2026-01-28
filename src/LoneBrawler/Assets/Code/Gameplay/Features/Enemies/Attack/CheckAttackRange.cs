// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common;
using Code.Gameplay.Features.Common;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAttack))]
  public class CheckAttackRange : MonoBehaviour, IDeactivatable, IActivatable
  {
    public EnemyAttack attack;
    public TriggerObserver triggerObserver;
    private bool _isActive;

    public void Deactivate()
    {
      attack.Deactivate();
      _isActive = false;
      enabled = false;
    }

    public void Activate()
    {
      _isActive = true;
      enabled = true;
    }

    private void Awake()
    {
      Activate();

      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit += HandleTriggerExit;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      if (_isActive)
      {
        attack.Activate();
      }
    }

    private void HandleTriggerExit(Collider collider)
    {
      attack.Deactivate();
    }

    private void OnDestroy()
    {
      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit -= HandleTriggerExit;
    }

  }
}
