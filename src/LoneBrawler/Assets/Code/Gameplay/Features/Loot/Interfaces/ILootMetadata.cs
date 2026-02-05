// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILootMetadata
  {
    void Construct(IGameConfigSubservice gameConfig);
  }
}