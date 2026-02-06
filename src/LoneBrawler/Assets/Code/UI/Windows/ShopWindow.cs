// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using TMPro;

namespace Code.UI.Windows
{
  public class ShopWindow : WindowBase
  {
    public TextMeshProUGUI currencyText;
    private ILootTrackerService _lootTracker;

    public override void Construct(IPersistentProgressService progressService)
    {
      base.Construct(progressService);
      _lootTracker = RootContext.Resolve<ILootTrackerService>();
    }

    protected override void Initialize() => RefreshCurrency();

    protected override void SubscribeUpdates() =>
      _lootTracker.OnValueChanged += RefreshCurrency;

    protected override void Cleanup()
    {
      base.Cleanup();
      _lootTracker.OnValueChanged -= RefreshCurrency;
    }

    private void RefreshCurrency() =>
      currencyText.text = _lootTracker.Souls.ToString();
  }
}
