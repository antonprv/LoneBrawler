// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;

using R3;

namespace Code.Infrastructure.Services.SoulsTracker
{
  public class SoulsTrackerService : ISoulsTrackerService
  {
    private IPersistentProgressService _progressService;

    private ReactiveProperty<int> _soulsRP = new(0);
    public ReadOnlyReactiveProperty<int> SoulsRP => _soulsRP;

    public SoulsTrackerService(IPersistentProgressService persistentProgress) =>
      _progressService = persistentProgress;

    public void AddSouls(int amount)
    {
      _progressService.Progress.SoulsCollected.Amount += amount;
      _soulsRP.Value = _progressService.Progress.SoulsCollected.Amount;
    }

    public bool TrySpendSouls(int amount)
    {
      int current = _progressService.Progress.SoulsCollected.Amount;
      if (current < amount) return false;

      _progressService.Progress.SoulsCollected.Amount -= amount;
      _soulsRP.Value = _progressService.Progress.SoulsCollected.Amount;
      return true;
    }

    public void ReadProgress(GameProgress playerProgress) =>
      _soulsRP.Value = playerProgress.SoulsCollected.Amount;
  }
}
