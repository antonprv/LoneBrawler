// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Common;

namespace Code.Gameplay.Features.Enemies.Movement
{
  public interface IMovableAgent : IDeactivatable
  {
    public void ReturnToStartPosition();
    public void StopFollowingImmediately();
    public void ContinueFollowing();
  }
}
