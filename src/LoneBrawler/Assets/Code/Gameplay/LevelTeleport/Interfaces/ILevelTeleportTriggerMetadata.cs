// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Assets.Code.Gameplay.LevelTeleport.Interfaces
{
  public interface ILevelTeleportTriggerMetadata
  {
    void Construct(IGameConfigSubservice gameConfig);
  }
}
