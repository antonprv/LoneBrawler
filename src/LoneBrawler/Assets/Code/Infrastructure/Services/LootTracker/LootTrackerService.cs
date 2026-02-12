// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

namespace Code.Infrastructure.Services.LootTracker
{
  public class LootTrackerService : ILootTrackerService
  {
    private IPersistentProgressService _persistentProgress;

    public int Souls
    {
      get => _persistentProgress.Progress.SoulsCollected.Amount;

      set
      {
        if (value == _persistentProgress.Progress.SoulsCollected.Amount)
          return;

        _persistentProgress.Progress.SoulsCollected.Amount = value;
        OnValueChanged?.Invoke();
      }
    }

    public event Action OnValueChanged;

    public LootTrackerService()
    {
      _persistentProgress = RootContext.Resolve<IPersistentProgressService>();
    }
  }
}
