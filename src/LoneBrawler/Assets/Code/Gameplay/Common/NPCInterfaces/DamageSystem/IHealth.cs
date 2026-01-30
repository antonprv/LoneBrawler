// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Common.NPCInterfaces.Animations;

namespace Code.Gameplay.Common.NPCInterfaces.DamageSystem
{
  public interface IHealth
  {
    public void Construct(IAnimator animator);

    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }

    event Action OnHealthChanged;

    void TakeDamage(float damage);
  }
}
