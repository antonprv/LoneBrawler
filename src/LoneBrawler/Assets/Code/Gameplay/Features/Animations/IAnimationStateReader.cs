// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace CodeBase.Logic
{
  public interface IAnimationStateReader
  {
    public void EnteredState(int stateHash);
    public void ExitedState(int stateHash);
    AnimatorState State { get; }
  }
}
