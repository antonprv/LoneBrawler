// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Features.Common
{
  public interface IHealth : IDeactivatable
  {
    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }

    event Action OnHealthChanged;

    void TakeDamage(float damage);
  }
}
