// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Gameplay.Utils.ActorComponents;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies.Movement
{
  [RequireComponent(typeof(NavMeshAgent))]
  public class MoveToPlayer : AsyncStartMonoBehaviour, IMovableAgent
  {
    // set in editor
    public NavMeshAgent agent;

    private float _reachDistance;
    private float _speed;
    private float _angularSpeed;
    private IGameLog _logger;
    private GameObject _player;
    private IAttacker _attacker;

    private Vector3 _initialPosition;
    private bool _canFollowPlayer;
    private bool _isActive;
    private bool _isAttacking;
    private bool _isInitialized;

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
      _logger = RootContext.Resolve<IGameLog>();

      _player = playerReader.GetPlayer();

      agent.speed = _speed;
      agent.angularSpeed = _angularSpeed;

      agent.updatePosition = true;
      agent.updateRotation = true;

      _attacker = attacker;
      _initialPosition = gameObject.transform.position;
    }

    protected override void AsyncStart()
    {
      if (agent == null)
      {
        _logger.Log(LogType.Error,
          $"{nameof(NavMeshAgent)} is missing on {gameObject.name}");
        return;
      }
      agent.ResetPath();
      agent.isStopped = true;

      SubscribeToAttacker();
    }

    protected override void VerifiedUpdate()
    {
      if (!PlayerNotReached() || !IsCurrentlyActive())
      {
        agent.isStopped = true;
        return;
      }

      FollowPlayer();
    }

    public void Activate()
    {
      _isActive = true;
      _canFollowPlayer = true;
    }

    public void Deactivate()
    {
      UnsubscribeFromAttacker();
      _isActive = false;
    }

    private void OnDestroy()
    {
      if (!_isInitialized) return;

      UnsubscribeFromAttacker();
    }

    private bool PlayerNotReached()
    {
      if (_player == null) return false;

      return Vector3.Distance(
        gameObject.transform.position,
        _player.transform.position) > _reachDistance;
    }

    private bool IsCurrentlyActive() => _canFollowPlayer && _isActive;

    private void HandleAttacking() => _isAttacking = true;
    private void HandleAttackFinished() => _isAttacking = false;

    private void SubscribeToAttacker()
    {
      _attacker.OnAttacking += HandleAttacking;
      _attacker.OnAttackFinished += HandleAttackFinished;
    }

    private void UnsubscribeFromAttacker()
    {
      if (!_isActive || _attacker == null) return;
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
