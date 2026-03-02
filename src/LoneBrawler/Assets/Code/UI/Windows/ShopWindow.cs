// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.SoulsTracker.Interfaces;

using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using R3;

using TMPro;

using Zenjex.Extensions.Core;
using Code.UI.Windows.Types;

namespace Code.UI.Windows
{
  public class ShopWindow : WindowBase
  {
    public TextMeshProUGUI currencyText;
    private ISoulsTrackerService _lootTracker;

    private CompositeDisposable _disposables;

    public override void Construct(IPersistentProgressService progressService, ConstructorContext context) =>
      base.Construct(progressService, context);

    protected override void InjectDependencies() =>
      _lootTracker = RootContext.Resolve<ISoulsTrackerService>();

    protected override void Initialize() => _disposables = new CompositeDisposable();

    protected override void SubscribeUpdates()
    {
      _lootTracker.SoulsRP
        .Subscribe(souls => currencyText.text = souls.ToString())
        .AddTo(_disposables);
    }

    protected override void Cleanup() => _disposables.Dispose();

  }
}
