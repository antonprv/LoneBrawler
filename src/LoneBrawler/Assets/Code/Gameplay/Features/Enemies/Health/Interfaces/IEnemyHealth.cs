// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;

namespace Code.Gameplay.Features.Enemies.Health.Interfaces
{
  public interface IEnemyHealth : IHealth, IEnemyStaticDataReceiver
  {
  }
}
