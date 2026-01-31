// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.Lifetime;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies.Animations
{
  [RequireComponent(typeof(EnemyAnimator))]
  [RequireComponent(typeof(NavMeshAgent))]
  public class AnimateAlongAgent : MonoBehaviour, IDeactivatable, IActivatable
  {
    public NavMeshAgent agent;
    public EnemyAnimator animator;
    private bool _isActive;

    private void Awake() => Activate();

    private void Update()
    {
      if (!_isActive) return;

      if (ShouldMove())
      {
        animator.Move(agent.desiredVelocity.magnitude);
      }
      else
      {
        animator.StopMoving();
      }
    }

    private bool ShouldMove()
    {
      return !agent.isStopped && agent.remainingDistance > agent.radius;
    }
    public void Activate()
    {
      _isActive = true;
    }

    public void Deactivate()
    {
      _isActive = false;
      enabled = false;
    }

  }
}
