// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;
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

    private bool ShouldMove() =>
      !agent.desiredVelocity.magnitude.IsNearlyZero()
      && agent.remainingDistance > agent.radius;

    public void Activate()
    {
      _isActive = true;
      enabled = true;
    }

    public void Deactivate()
    {
      _isActive = false;
      enabled = false;
    }

  }
}
