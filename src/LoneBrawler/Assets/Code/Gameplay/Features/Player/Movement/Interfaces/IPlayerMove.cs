// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Movement.Interfaces
{
  public interface IPlayerMove : IDeactivatable, IActivatable
  {
    public void Construct(IAttacker attacker);
    public void Warp(Vector3 to);
  }
}
