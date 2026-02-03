// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Common.NPCInterfaces.Lifetime;

namespace Code.Gameplay.Common.NPCInterfaces.DamageSystem
{
  public interface IAttacker : IDeactivatable
  {
    public event Action OnAttacking;
    public event Action OnAttackFinished;
  }

  public interface IPlayerAttacker : IAttacker
  {
  }
}
