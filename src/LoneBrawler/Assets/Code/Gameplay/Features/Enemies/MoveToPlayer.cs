// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies
{
  public class MoveToPlayer : MonoBehaviour
  {
    public NavMeshAgent agent;

    public float reachDistance = 1f;
    private IGameLog _logger;
    private IPlayerProvider _playerProvider;
    private GameObject _player;

    private void Awake()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _playerProvider = RootContext.Resolve<IPlayerProvider>();
    }

    private void Update()
    {
      if (_player == null)
      {
        _playerProvider.OnPlayerSet += HandlePlayerSet;

        _logger.Log($"{nameof(IPlayerProvider)} 2 hash"
          + _playerProvider.GetHashCode().ToString());
        return;
      }

      if (PlayerNotReached())
        FollowPlayer();
    }

    private void OnDisable()
    {
      _playerProvider.OnPlayerSet -= HandlePlayerSet;
    }

    private void HandlePlayerSet(GameObject player)
    {
      _player = player;
    }

    private void FollowPlayer()
    {
      agent.destination = _player.transform.position;
    }

    private bool PlayerNotReached()
    {
      if (_player == null) return true;

      return Vector3.Distance(gameObject.transform.position, _player.transform.position) > reachDistance;
    }

  }
}
