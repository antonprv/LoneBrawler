// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Gameplay.Features.Player.Metadata.Interfaces
{
  public interface IPlayerMetadata
  {
    void Construct(IGameConfigSubservice gameConfigSubservice);
  }
}
