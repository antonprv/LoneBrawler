// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Common.NPCInterfaces
{
  public interface IAttacker : IDeactivatable
  {
    public event Action OnAttacking;
    public event Action OnAttackFinished;

    float AttackRange { get; }
    float AttackRadius { get; }
    float Damage { get; }
    int MaxHit { get; }
  }
}
