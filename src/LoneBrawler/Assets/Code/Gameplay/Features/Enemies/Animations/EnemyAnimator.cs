// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Features.AnimationsCommon;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Animations
{
  public class EnemyAnimator : MonoBehaviour, IAnimationStateReader
  {
    public Animator animator;

    public event Action<AnimatorState> OnStateEnter;
    public event Action<AnimatorState> OnStateExit;

    public AnimatorState State { get; private set; }

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Win = Animator.StringToHash("Win");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int PointAttack = Animator.StringToHash("PointAttack");
    private static readonly int AreaAttack = Animator.StringToHash("AreaAttack");

    // Transitioned states
    private readonly int _idleStateHash = Animator.StringToHash("Idle");
    private readonly int _moveStateHash = Animator.StringToHash("MoveBlendTree");
    private readonly int _attack01StateHash = Animator.StringToHash("PointAttack");
    private readonly int _attack02StateHash = Animator.StringToHash("AreaAttack");

    // Transitioned any states
    private readonly int _getHitStateHash = Animator.StringToHash("GetHit");
    private readonly int _victoryStateHash = Animator.StringToHash("Victory");
    private readonly int _deathStateHash = Animator.StringToHash("Death");


    private bool _isDead = false;

    public void Move(float speed)
    {
      animator.SetBool(IsMoving, true);
      animator.SetFloat(Speed, speed);
    }

    public void StopMoving() => animator.SetBool(IsMoving, false);

    public void PlayDeath()
    {
      _isDead = true;
      animator.SetTrigger(Die);
    }

    public void PlayWin() => animator.SetTrigger(Win);
    public void PlayHit()
    {
      if (_isDead) return;
      animator.SetTrigger(Hit);
    }

    public void PlayPointAttack() => animator.SetTrigger(PointAttack);
    public void PlayAreaAttack() => animator.SetTrigger(AreaAttack);

    public void EnteredState(int stateHash)
    {
      State = StateFor(stateHash);
      OnStateEnter?.Invoke(State);
    }

    public void ExitedState(int stateHash) =>
      OnStateExit?.Invoke(StateFor(stateHash));

    private AnimatorState StateFor(int stateHash)
    {
      AnimatorState state;
      if (stateHash == _idleStateHash)
        state = AnimatorState.Idle;
      else if (stateHash == _attack01StateHash || stateHash == _attack02StateHash)
        state = AnimatorState.Attack;
      else if (stateHash == _moveStateHash)
        state = AnimatorState.Walking;
      else if (stateHash == _deathStateHash)
        state = AnimatorState.Died;
      else
        state = AnimatorState.Unknown;

      return state;
    }
  }
}
