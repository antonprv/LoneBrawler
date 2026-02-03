// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Services.Input.Interfaces;

namespace Code.Gameplay.Features.Player.Movement.Interfaces
{
  public interface IPlayerMove
  {
    void Construct(IInputService inputService, ITimeService timeService, IAttacker attacker);
  }
}
