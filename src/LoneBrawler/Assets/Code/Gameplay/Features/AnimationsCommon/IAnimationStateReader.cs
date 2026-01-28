// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Features.AnimationsCommon
{
  public interface IAnimationStateReader
  {
    public void EnteredState(int stateHash);
    public void ExitedState(int stateHash);
    AnimatorState State { get; }
  }
}
