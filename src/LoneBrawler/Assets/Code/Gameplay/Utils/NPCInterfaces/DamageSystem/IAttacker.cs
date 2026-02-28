// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

namespace Code.Gameplay.Utils.NPCInterfaces.DamageSystem
{
  public interface IAttacker : IDeactivatable
  {
    public event Action OnAttacking;
    public event Action OnAttackFinished;
  }

  public interface IPlayerAttacker : IAttacker
  {
    public void Construct(IAnimator animator);
  }
}
