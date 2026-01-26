// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;

using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.Features.Enemies.Animations
{
  [RequireComponent(typeof(EnemyAnimator))]
  [RequireComponent(typeof(NavMeshAgent))]
  public class AnimateAlongAgent : MonoBehaviour
  {
    public NavMeshAgent agent;
    public EnemyAnimator animator;

    private void Update()
    {
      if (ShouldMove())
      {
        animator.Move(agent.velocity.GetLengthXZ());
      }
      else
      {
        animator.StopMoving();
      }
    }

    private bool ShouldMove()
    {
      return agent.velocity.GetLengthXZ() > Constants.KINDA_SMALL_NUMBER
        && agent.remainingDistance > agent.radius;
    }
  }
}
