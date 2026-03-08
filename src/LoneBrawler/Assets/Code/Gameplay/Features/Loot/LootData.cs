// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Loot.Interfaces;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Loot
{
  public class LootData : ZenjexBehaviour, ILootData
  {
    [Zenjex] private readonly ISoulsTrackerService _soulsTracker;

    private ILoot _loot;
    private CompositeDisposable _disposables = new();

    public void Construct(ILoot loot)
    {
      _loot = loot;
      _disposables = new CompositeDisposable();

      _loot.OnCollected
        .Take(1)
        .Subscribe(_ => _soulsTracker.AddSouls(_loot.Souls))
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables?.Dispose();
  }
}
