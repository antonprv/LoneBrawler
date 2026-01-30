// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Movement
{
  public class RotateToPlayer : MonoBehaviour, IMovableAgent
  {
    public float Speed { get; set; }
    public float AngularSpeed { get; set; }

    private GameObject _player;
    private ITimeService _timeService;
    private IAttacker _attacker;
    private Quaternion _initialRotation;

    private bool _canFollowPlayer;

    private Quaternion _targetRotation;
    private bool _isActive;
    private bool _isAttacking;

    public void Construct(IPlayerReader playerReader, IEnemyAttacker attacker)
    {
      _timeService = RootContext.Resolve<ITimeService>();
      _player = playerReader.Player;

      _attacker = attacker;
      SubscribeToAttacker();

      _initialRotation = gameObject.transform.rotation;
      Activate();
    }

    private void HandleAttacking() => _isAttacking = true;
    private void HandleAttackFinished() => _isAttacking = false;


    private void Update()
    {
      if (IsInactive()) return;
      RotateSelf();
    }

    private void OnDestroy() => UnsubscribeFromAttacker();

    private bool IsInactive() =>
      !_isActive
      || (!_canFollowPlayer
      && transform.rotation.IsNearlyEqual(_initialRotation));

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
          AngularSpeed * _timeService.DeltaAt60FPS * Speed
      );
    }

    private void Activate() => _isActive = true;

    public void Deactivate()
    {
      UnsubscribeFromAttacker();
      _isActive = false;
    }

    private void SubscribeToAttacker()
    {
      _attacker.OnAttacking += HandleAttacking;
      _attacker.OnAttackFinished += HandleAttackFinished;
    }

    private void UnsubscribeFromAttacker()
    {
      if (!_isActive) return;
      _attacker.OnAttacking -= HandleAttacking;
      _attacker.OnAttackFinished -= HandleAttackFinished;
    }

    public void ReturnToStartPosition()
    {
      StopFollowingImmediately();
      _targetRotation = _initialRotation;
    }

    public void StopFollowingImmediately() => _canFollowPlayer = false;

    public void ContinueFollowing() => _canFollowPlayer = true;



  }
}
