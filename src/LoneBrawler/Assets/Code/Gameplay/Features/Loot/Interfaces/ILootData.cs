// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.LootTracker.Interfaces;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILootData
  {
    void Construct(ILoot loot, ILootTrackerService lootTracker);
  }
}
