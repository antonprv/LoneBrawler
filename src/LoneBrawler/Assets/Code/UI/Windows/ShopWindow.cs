// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using R3;

using TMPro;

using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class ShopWindow : WindowBase
  {
    public TextMeshProUGUI currencyText;
    private ILootTrackerService _lootTracker;

    private CompositeDisposable _disposables;

    public override void Construct(IPersistentProgressService progressService) =>
      base.Construct(progressService);

    protected override void InjectDependencies() =>
      _lootTracker = RootContext.Resolve<ILootTrackerService>();

    protected override void Initialize()
    {
      _disposables = new CompositeDisposable();
      currencyText.text = string.Empty;
    }

    protected override void SubscribeUpdates()
    {
      _lootTracker.SoulsRP
        .Subscribe(souls => currencyText.text = souls.ToString())
        .AddTo(_disposables);
    }

    protected override void Cleanup() => _disposables.Dispose();

  }
}
