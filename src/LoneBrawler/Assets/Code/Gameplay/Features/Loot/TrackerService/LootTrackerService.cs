// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Features.Loot.TrackerService.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

namespace Code.Gameplay.Features.Loot.TrackerService
{
  public class LootTrackerService : ILootTrackerService
  {
    private IPersistentProgressService _persistentProgress;

    public int Souls
    {
      get => _persistentProgress.Progress.SoulsCollected.Amount;

      set
      {
        _persistentProgress.Progress.SoulsCollected.Amount += value;
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
