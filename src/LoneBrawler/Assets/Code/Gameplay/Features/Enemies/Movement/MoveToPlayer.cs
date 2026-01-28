// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Features.Common;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies.Movement
{
  [RequireComponent(typeof(NavMeshAgent))]
  public class MoveToPlayer : MonoBehaviour, IMovableAgent, IActivatable
  {
    public NavMeshAgent agent;
    public float reachDistance = 1f;

    private GameObject _player;
    private IPlayerReader _playerReader;
    private Vector3 _initialPosition;
    private bool _canFollowPlayer;
    private bool _isActive;

    private void Awake()
    {
      Activate();
      _playerReader = RootContext.Resolve<IPlayerReader>();
    }

    private void Start()
    {
      _initialPosition = gameObject.transform.position;
    }

    private void Update()
    {
      if (!_canFollowPlayer || !_isActive) return;

      if (_player == null)
      {
        _player = _playerReader.Player;
        return;
      }

      if (PlayerNotReached())
        FollowPlayer();
    }

    public void ReturnToStartPosition()
    {
      agent.destination = _initialPosition;
    }

    public void StopFollowingImmediately()
    {
      _canFollowPlayer = false;
      agent.destination = gameObject.transform.position;
    }

    public void ContinueFollowing()
    {
      _canFollowPlayer = true;
    }

    private void FollowPlayer()
    {
      agent.destination = _player.transform.position;
    }

    private bool PlayerNotReached()
    {
      if (_player == null) return false;

      return Vector3.Distance(
        gameObject.transform.position,
        _player.transform.position) > reachDistance;
    }

    public void Deactivate()
    {
      _isActive = false;
      enabled = false;
    }

    public void Activate() => _isActive = true;
  }
}
