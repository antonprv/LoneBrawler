// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;

namespace Code.Gameplay.Features.Enemies.Health.Interfaces
{
  public interface IEnemyDeath : IDeath, IEnemyStaticDataReceiver
  {
    public event Action OnDead;
  }
}
