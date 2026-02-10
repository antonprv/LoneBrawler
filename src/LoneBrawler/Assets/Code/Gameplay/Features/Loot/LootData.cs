// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Loot.Interfaces;
using Code.Infrastructure.Services.LootTracker.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
  public class LootData : MonoBehaviour, ILootData
  {
    private ILoot _loot;
    private ILootTrackerService _lootTracker;

    public void Construct(ILoot loot, ILootTrackerService lootTracker)
    {
      _loot = loot;
      _lootTracker = lootTracker;

      _loot.OnCollected += HandleCollected;
    }

    private void HandleCollected() => GiveSoulsToPlayer();
    private void GiveSoulsToPlayer() => _lootTracker.Souls += _loot.Souls;
  }
}
