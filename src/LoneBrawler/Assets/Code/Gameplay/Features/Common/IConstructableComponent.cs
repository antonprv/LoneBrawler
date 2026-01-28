// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Common;

namespace Assets.Code.Gameplay.Features.Common
{
  public interface IConstructableComponent : IDeactivatable
  {
    void Initialize();
  }
}
