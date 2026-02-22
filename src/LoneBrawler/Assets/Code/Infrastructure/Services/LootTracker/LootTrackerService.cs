// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using R3;

namespace Code.Infrastructure.Services.LootTracker
{
  public class LootTrackerService : ILootTrackerService
  {
    private IPersistentProgressService _persistentProgress;

    private ReactiveProperty<int> _soulsRP = new(0);
    public ReadOnlyReactiveProperty<int> SoulsRP => _soulsRP;

    public LootTrackerService(IPersistentProgressService persistentProgress) =>
      _persistentProgress = persistentProgress;

    public void AddSouls(int amount)
    {
      _persistentProgress.Progress.SoulsCollected.Amount += amount;
      _soulsRP.Value = _persistentProgress.Progress.SoulsCollected.Amount;
    }

    public bool SpendSouls(int amount)
    {
      int current = _persistentProgress.Progress.SoulsCollected.Amount;
      if (current < amount) return false;

      _persistentProgress.Progress.SoulsCollected.Amount -= amount;
      _soulsRP.Value = _persistentProgress.Progress.SoulsCollected.Amount;
      return true;
    }
  }
}
