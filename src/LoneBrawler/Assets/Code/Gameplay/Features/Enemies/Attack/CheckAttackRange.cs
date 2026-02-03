// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAttack))]
  public class CheckAttackRange : MonoBehaviour, ICheckAttackRange
  {
    public TriggerObserver triggerObserver;

    private IEnemyAttacker _attacker;
    private bool _isActive;

    public void Construct(IEnemyAttacker attacker)
    {
      _attacker = attacker;
      SubscribeToTriggers();
      Activate();
    }

    private void OnDestroy() => UnsubscribeFromTriggers();

    public void Activate() => _isActive = true;

    public void Deactivate()
    {
      _attacker.Deactivate();
      _isActive = false;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      if (_isActive)
        _attacker.Activate();
    }

    private void HandleTriggerExit(Collider collider) => _attacker.Deactivate();


    private void SubscribeToTriggers()
    {
      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit += HandleTriggerExit;
    }
    private void UnsubscribeFromTriggers()
    {
      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;
      triggerObserver.ObservedOnTriggerExit -= HandleTriggerExit;
    }
  }
}
