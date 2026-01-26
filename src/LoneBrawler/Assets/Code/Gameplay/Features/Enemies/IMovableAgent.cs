// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Features.Enemies
{
  public interface IMovableAgent
  {
    public void ReturnToStartPosition();
    public void StopFollowingImmediately();
    public void ContinueFollowing();
  }
}
