// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

namespace Code.Gameplay.Features.Enemies.Movement.Interfaces
{
  public interface IMovableAgent : IDeactivatable, ISettableStaticData
  {
    public void Construct(
      IPlayerReader playerReader,
      IEnemyAttacker attacker);

    public void ReturnToStartPosition();
    public void StopFollowingImmediately();
    public void ContinueFollowing();
  }
}
