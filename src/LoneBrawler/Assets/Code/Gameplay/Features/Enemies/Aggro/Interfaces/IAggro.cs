// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;

namespace Code.Gameplay.Features.Enemies.Aggro.Interfaces
{
  public interface IAggro : IActivatable, IDeactivatable
  {
    void Construct(IMovableAgent movableAgent);
  }
}
