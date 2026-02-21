// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.Interfaces
{
  public interface IEnemyAttacker : IAttacker, IEnemyStaticDataReceiver
  {
    public void StartAttacking();

    public void Construct(
      GameObject player,
      IAnimator animator,
      IDeath playerDeath,
      IHealth playerHealth,
      IHealth enemyHealth,
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig
      );
  }
}
