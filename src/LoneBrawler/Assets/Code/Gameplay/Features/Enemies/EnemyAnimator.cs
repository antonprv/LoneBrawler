// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Features.Animations;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies
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
    private static readonly int Attack1 = Animator.StringToHash("Attack1");
    private static readonly int Attack2 = Animator.StringToHash("Attack2");

    // Transitioned states
    private readonly int _idleStateHash = Animator.StringToHash("idle");
    private readonly int _moveStateHash = Animator.StringToHash("moveBlendTree");
    private readonly int _attack01StateHash = Animator.StringToHash("attack01");
    private readonly int _attack02StateHash = Animator.StringToHash("attack02");

    // Transitioned any states
    private readonly int _getHitStateHash = Animator.StringToHash("getHit");
    private readonly int _victoryStateHash = Animator.StringToHash("victory");
    private readonly int _dieStateHash = Animator.StringToHash("die");

    public void Move(float speed)
    {
      animator.SetBool(IsMoving, true);
      animator.SetFloat(Speed, speed);
    }

    public void StopMoving() => animator.SetBool(IsMoving, false);

    public void PlayDie() => animator.SetTrigger(Die);
    public void PlayWin() => animator.SetTrigger(Win);
    public void PlayHit() => animator.SetTrigger(Hit);
    public void PlayAttack1() => animator.SetTrigger(Attack1);
    public void PlayAttack2() => animator.SetTrigger(Attack2);

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
      else if (stateHash == _dieStateHash)
        state = AnimatorState.Died;
      else
        state = AnimatorState.Unknown;

      return state;
    }
  }
}
