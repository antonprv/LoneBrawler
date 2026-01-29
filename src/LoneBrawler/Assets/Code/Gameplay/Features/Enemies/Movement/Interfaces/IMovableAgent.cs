// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces;

namespace Code.Gameplay.Features.Enemies.Movement.Interfaces
{
  public interface IMovableAgent : IDeactivatable
  {
    public void ReturnToStartPosition();
    public void StopFollowingImmediately();
    public void ContinueFollowing();
  }
}
