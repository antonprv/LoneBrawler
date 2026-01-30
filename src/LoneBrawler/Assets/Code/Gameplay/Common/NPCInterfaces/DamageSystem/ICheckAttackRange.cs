// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;

namespace Code.Gameplay.Features.Enemies.Attack
{
  public interface ICheckAttackRange : IDeactivatable
  {
    void Construct(IEnemyAttacker attacker);
  }
}
