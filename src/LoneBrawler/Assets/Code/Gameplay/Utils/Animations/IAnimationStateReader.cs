// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Utils.Animations
{
  public interface IAnimationStateReader
  {
    public void EnteredState(int stateHash);
    public void ExitedState(int stateHash);
    AnimatorState State { get; }
  }
}
