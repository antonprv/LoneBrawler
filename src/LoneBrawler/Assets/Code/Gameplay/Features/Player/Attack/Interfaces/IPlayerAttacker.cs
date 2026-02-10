// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

namespace Code.Gameplay.Features.Player.Attack.Interfaces
{
  public interface IPlayerAttacker
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
