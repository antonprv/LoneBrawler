// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies.Movement
{
  [RequireComponent(typeof(NavMeshAgent))]
  public class MoveToPlayer : MonoBehaviour, IMovableAgent
  {
    public NavMeshAgent agent;
    public float reachDistance = 1f;

    public float Speed { get; set; }
    public float AngularSpeed { get; set; }

    private GameObject _player;
    private IAttacker _attacker;

    private Vector3 _initialPosition;
    private bool _canFollowPlayer;
    private bool _isActive;
    private bool _isAttacking;

    public void Construct(IPlayerReader playerReader, IEnemyAttacker attacker)
    {
      _player = playerReader.Player;

      agent.speed = Speed;
      agent.angularSpeed = AngularSpeed;

      _attacker = attacker;
      SubscribeToAttacker();

      _initialPosition = gameObject.transform.position;
      Activate();
    }

    private void Update()
    {
      if (PlayerNotReached() && IsCurrentlyActive())
        FollowPlayer();
    }
    private void OnDestroy()
    {
      UnsubscribeFromAttacker();
    }

    private bool PlayerNotReached() => Vector3.Distance(
        gameObject.transform.position,
        _player.transform.position) > reachDistance;

    private bool IsCurrentlyActive() => _canFollowPlayer && _isActive;

    private void HandleAttacking() => _isAttacking = true;
    private void HandleAttackFinished() => _isAttacking = false;

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
    private void FollowPlayer()
    {
      agent.destination =
        _isAttacking ?
        transform.position : _player.transform.position;
    }

    public void ReturnToStartPosition() => agent.destination = _initialPosition;

    public void StopFollowingImmediately()
    {
      _canFollowPlayer = false;
      agent.destination = gameObject.transform.position;
    }

    public void ContinueFollowing() => _canFollowPlayer = true;


  }
}
