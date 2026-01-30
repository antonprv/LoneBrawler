// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Common.NPCInterfaces.DamageSystem
{
  public interface IAttacker : IDeactivatable
  {
    public event Action OnAttacking;
    public event Action OnAttackFinished;

    float Range { get; set; }
    float Radius { get; set; }
    float Damage { get; set; }
    int MaxHit { get; set; }
  }

  public interface IEnemyAttacker : IAttacker, IActivatable
  {
    public void Construct(
      GameObject player,
      IAnimator animator,
      IDeath playerDeath,
      IHealth playerHealth,
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig
      );

    public float Cooldown { get; set; }
    public float TurnSpeed { get; set; }
  }

  public interface IPlayerAttacker : IAttacker
  {
  }
}
