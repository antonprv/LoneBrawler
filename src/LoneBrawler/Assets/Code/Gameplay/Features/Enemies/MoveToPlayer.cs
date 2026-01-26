// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies
{
  [RequireComponent(typeof(NavMeshAgent))]
  public class MoveToPlayer : MonoBehaviour, IMovableAgent
  {
    public NavMeshAgent agent;
    public float reachDistance = 1f;

    private GameObject _player;
    private IPlayerReader _playerReader;
    private Vector3 _initialPosition;
    private bool _canFollow;

    private void Awake()
    {
      _playerReader = RootContext.Resolve<IPlayerReader>();
    }

    private void Start()
    {
      _initialPosition = gameObject.transform.position;
    }

    private void Update()
    {
      if (_player == null)
      {
        _player = _playerReader.Player;
        return;
      }

      if (PlayerNotReached() && _canFollow)
        FollowPlayer();
    }

    public void ReturnToStartPosition()
    {
      agent.destination = _initialPosition;
    }

    public void StopMovingImmediately()
    {
      _canFollow = false;
      agent.destination = gameObject.transform.position;
    }

    public void ContinueFollowing()
    {
      _canFollow = true;
    }

    private void FollowPlayer()
    {
      agent.destination = _player.transform.position;
    }

    private bool PlayerNotReached()
    {
      if (_player == null) return true;

      return Vector3.Distance(
        gameObject.transform.position,
        _player.transform.position) > reachDistance;
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
