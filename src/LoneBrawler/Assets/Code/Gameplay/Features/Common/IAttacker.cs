// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Features.Common
{
  public interface IAttacker : IDeactivatable
  {
    float AttackRange { get; }
    float AttackRadius { get; }
    float Damage { get; }
    int MaxHit { get; }
  }
}
