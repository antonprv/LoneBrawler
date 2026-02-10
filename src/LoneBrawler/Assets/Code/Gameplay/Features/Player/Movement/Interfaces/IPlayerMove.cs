// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.Time;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Movement.Interfaces
{
  public interface IPlayerMove : IDeactivatable, IActivatable
  {
    public void Construct(IInputService inputService, ITimeService timeService, IAttacker attacker);
    public void Warp(Vector3 to);
  }
}
