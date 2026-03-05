// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

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
    private bool _shouldMove;

    public void Construct(EnemyStaticData staticData) =>
      _shouldMove = staticData.ShouldMove;

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
      if (!_shouldMove) return;

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
