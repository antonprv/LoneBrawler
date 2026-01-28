// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Common;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Movement
{
  public class RotateToPlayer : MonoBehaviour, IMovableAgent, IActivatable
  {
    public float Speed = 1f;

    private GameObject _player;
    private IPlayerReader _playerReader;
    private ITimeService _timeService;
    private IAttacker _attacker;
    private Quaternion _initialRotation;

    private bool _canFollowPlayer;

    private Quaternion _targetRotation;
    private bool _isActive;
    private bool _isAttacking;

    private void Awake()
    {
      Activate();

      _playerReader = RootContext.Resolve<IPlayerReader>();
      _timeService = RootContext.Resolve<ITimeService>();

      _attacker = GetComponent<IAttacker>();
      _attacker.OnAttacking += HandleAttacking;
      _attacker.OnAttackFinished += HandleAttackFinished;
    }

    private void HandleAttacking() => _isAttacking = true;

    private void HandleAttackFinished() => _isAttacking = false;

    private void Start()
    {
      _initialRotation = gameObject.transform.rotation;
    }

    private void Update()
    {
      if (IsInactive()) return;

      if (_player == null)
      {
        _player = _playerReader.Player;
        return;
      }

      RotateSelf();
    }

    private void OnDestroy()
    {
      _attacker.OnAttacking -= HandleAttacking;
      _attacker.OnAttackFinished -= HandleAttackFinished;
    }

    private void RotateSelf()
    {
      if (_isAttacking) return;

      if (!_canFollowPlayer)
      {
        _targetRotation = _initialRotation;
      }
      else
      {
        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < Constants.KINDA_SMALL_NUMBER)
          return;

        _targetRotation = Quaternion.LookRotation(direction);
      }

      transform.rotation = Quaternion.Slerp(
          transform.rotation,
          _targetRotation,
          Speed * _timeService.DeltaTime
      );
    }

    public void ReturnToStartPosition()
    {
      StopFollowingImmediately();
      _targetRotation = _initialRotation;
    }

    public void StopFollowingImmediately()
    {
      _canFollowPlayer = false;
    }

    public void ContinueFollowing()
    {
      _canFollowPlayer = true;
    }

    private bool IsInactive() =>
      !_isActive
      || (!_canFollowPlayer
      && transform.rotation.IsNearlyEqual(_initialRotation));

    public void Deactivate()
    {
      _isActive = false;
      enabled = false;
    }

    public void Activate() => _isActive = true;
  }
}
