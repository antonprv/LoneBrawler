// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.UtilityComponents;

using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Utils.ActorComponents;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Core;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAttack))]
  public class CheckAttackRange : AsyncStartMonoBehaviour, ICheckAttackRange
  {
    public TriggerObserver triggerObserver;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;

    private IEnemyAttacker _attacker;
    private int _playerLayer;

    private bool _isActive;

    public void Construct(IEnemyAttacker attacker)
    {
      _attacker = attacker;
      _playerLayer = _gameConfig.PlayerLayer;
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
      if (!_isActive) return;
      if (collider.gameObject.layer != _playerLayer) return;

      _attacker.StartAttacking();
    }

    private void HandleTriggerExit(Collider collider)
    {
      if (collider.gameObject.layer != _playerLayer) return;

      _attacker.Deactivate();
    }

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
