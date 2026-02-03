// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.Interfaces
{
  public interface IEnemyAttacker : IAttacker, IActivatable, IEnemyStaticDataReceiver
  {
    public void Construct(
      GameObject player,
      IAnimator animator,
      IDeath playerDeath,
      IHealth playerHealth,
      IEnemyHealth enemyHealth,
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig
      );
  }
}
