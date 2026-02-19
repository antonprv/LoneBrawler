// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.UtilityComponents;

using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Utils.ActorComponents;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAttack))]
  public class CheckAttackRange : AsyncStartMonoBehaviour, ICheckAttackRange
  {
    public TriggerObserver triggerObserver;
    private IGameLog _logger;
    private IEnemyAttacker _attacker;
    private bool _isActive;

    public void Construct(IEnemyAttacker attacker)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _attacker = attacker;
    }

    protected override void AsyncStart()
    {
      if (triggerObserver == null)
      {
        _logger.Log(LogType.Error,
          $"{nameof(triggerObserver)} is missing on {gameObject.name}");
        return;
      }
      SubscribeToTriggers();
    }

    private void OnDestroy()
    {
      if (!IsInitialized) return;

      UnsubscribeFromTriggers();
    }

    public void Activate()
    {
      _isActive = true;
      enabled = true;
    }

    public void Deactivate()
    {
      _attacker?.Deactivate();
      _isActive = false;
      enabled = false;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      if (_isActive)
        _attacker.StartAttacking();
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
