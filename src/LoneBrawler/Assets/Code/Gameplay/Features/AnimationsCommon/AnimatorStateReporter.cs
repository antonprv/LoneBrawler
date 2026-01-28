// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Features.AnimationsCommon
{
  public class AnimatorStateReporter : StateMachineBehaviour
  {
    private IAnimationStateReader _stateReader;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnStateEnter(animator, stateInfo, layerIndex);
      FindReader(animator);

      _stateReader.EnteredState(stateInfo.shortNameHash);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnStateExit(animator, stateInfo, layerIndex);
      FindReader(animator);

      _stateReader.ExitedState(stateInfo.shortNameHash);
    }

    private void FindReader(Animator animator)
    {
      if (_stateReader != null)
        return;

      _stateReader = animator.gameObject.GetComponent<IAnimationStateReader>();
    }
  }
}
