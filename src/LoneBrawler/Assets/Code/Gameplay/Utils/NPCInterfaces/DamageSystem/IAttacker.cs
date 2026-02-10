// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

namespace Code.Gameplay.Utils.NPCInterfaces.DamageSystem
{
  public interface IAttacker : IDeactivatable
  {
    public event Action OnAttacking;
    public event Action OnAttackFinished;
  }

  public interface IPlayerAttacker : IAttacker
  {
    public void Construct(
      IInputService inputService,
      ITimeService timeService,
      IGameConfigSubservice gameConfig,
      IBuildConfigSubservice buildConfig,
      IAnimator animator
      );
  }
}
