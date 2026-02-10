// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

namespace Code.Gameplay.Features.Enemies.Attack.Interfaces
{
  public interface ICheckAttackRange : IDeactivatable, IActivatable
  {
    void Construct(IEnemyAttacker attacker);
  }
}
