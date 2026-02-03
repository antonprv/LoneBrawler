// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies.Movement
{
  [RequireComponent(typeof(NavMeshAgent))]
  public class MoveToPlayer : MonoBehaviour, IMovableAgent
  {
    // set in editor
    public NavMeshAgent agent;

    private float _reachDistance;
    private float _speed;
    private float _angularSpeed;

    private GameObject _player;
    private IAttacker _attacker;

    private Vector3 _initialPosition;
    private bool _canFollowPlayer;
    private bool _isActive;
    private bool _isAttacking;

    public void SetValues(EnemyStaticData staticData)
    {
      _reachDistance = staticData.ReachDistance;
      _speed = staticData.Speed;
      _angularSpeed = staticData.AngularSpeed;
    }

    public void Construct(
      IPlayerReader playerReader,
      IEnemyAttacker attacker
      )
    {
      _player = playerReader.GetPlayer();

      agent.speed = _speed;
      agent.angularSpeed = _angularSpeed;

      agent.isStopped = true;
      agent.updatePosition = true;
      agent.updateRotation = true;

      _attacker = attacker;
      SubscribeToAttacker();

      _initialPosition = gameObject.transform.position;
      Activate();
    }

    private void Start() => agent.ResetPath();

    private void Update()
    {
      if (!PlayerNotReached() || !IsCurrentlyActive())
      {
        agent.isStopped = true;
        return;
      }

      FollowPlayer();
    }

    private void OnDestroy() => UnsubscribeFromAttacker();

    private bool PlayerNotReached() => Vector3.Distance(
        gameObject.transform.position,
        _player.transform.position) > _reachDistance;

    private bool IsCurrentlyActive() => _canFollowPlayer && _isActive;

    private void HandleAttacking() => _isAttacking = true;
    private void HandleAttackFinished() => _isAttacking = false;

    public void Activate()
    {
      _isActive = true;
      _canFollowPlayer = true;
      agent.isStopped = false;
    }

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
      if (_isAttacking)
      {
        agent.isStopped = true;
        return;
      }

      agent.isStopped = false;

      if (!agent.hasPath || agent.destination != _player.transform.position)
        agent.SetDestination(_player.transform.position);
    }


    public void ReturnToStartPosition() => agent.destination = _initialPosition;

    public void StopFollowingImmediately()
    {
      _canFollowPlayer = false;
      agent.isStopped = true;
      agent.ResetPath();
    }

    public void ContinueFollowing() => _canFollowPlayer = true;
  }
}
