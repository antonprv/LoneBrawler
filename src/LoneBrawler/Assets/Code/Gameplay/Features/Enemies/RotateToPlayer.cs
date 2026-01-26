// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies
{
  public class RotateToPlayer : MonoBehaviour, IMovableAgent
  {
    public float Speed = 1f;

    private GameObject _player;
    private IPlayerReader _playerReader;
    private ITimeService _timeService;
    private Quaternion _initialRotation;

    private bool _canFollowPlayer;

    private Quaternion _targetRotation;


    private void Awake()
    {
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _timeService = RootContext.Resolve<ITimeService>();
    }

    private void Start()
    {
      _initialRotation = gameObject.transform.rotation;
    }

    private void Update()
    {
      if (_player == null)
      {
        _player = _playerReader.Player;
        return;
      }

      RotateSelf();
    }

    private void RotateSelf()
    {
      if (!_canFollowPlayer)
      {
        _targetRotation = _initialRotation;
      }
      else
      {
        Vector3 dir = _player.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < Constants.KINDA_SMALL_NUMBER)
          return;

        _targetRotation = Quaternion.LookRotation(dir);
      }

      transform.rotation = Quaternion.Slerp(
          transform.rotation,
          _targetRotation,
          Speed * _timeService.DeltaTime
      );
    }


    public void ReturnToStartPosition()
    {
      StopMovingImmediately();
      _targetRotation = _initialRotation;
    }

    public void StopMovingImmediately()
    {
      _canFollowPlayer = false;
    }

    public void ContinueFollowing()
    {
      _canFollowPlayer = true;
    }

    public void DisableSelf()
    {
      enabled = false;
    }

    public void EnableSelf()
    {
      enabled = true;
    }
  }
}
