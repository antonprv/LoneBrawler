// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace CodeBase.Logic
{
  public interface IAnimationStateReader
  {
    void EnteredState(int stateHash);
    void ExitedState(int stateHash);
    AnimatorState State { get; }
  }
}
