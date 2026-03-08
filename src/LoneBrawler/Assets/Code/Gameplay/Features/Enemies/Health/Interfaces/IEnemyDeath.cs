// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using R3;

namespace Code.Gameplay.Features.Enemies.Health.Interfaces
{
  public interface IEnemyDeath : IDeath, IEnemyStaticDataReceiver
  {
    Observable<Unit> OnDead { get; }
  }
}
