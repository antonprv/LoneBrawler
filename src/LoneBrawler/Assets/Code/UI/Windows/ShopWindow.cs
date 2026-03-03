// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.UI.Windows.Types;

using R3;

using TMPro;

using UnityEngine.UI;

using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class ShopWindow : WindowBase
  {
    public TextMeshProUGUI currencyText;
    private ISoulsTrackerService _soulsTracker;

    public override WindowTypeId WindowType => WindowTypeId.Shop;

    private CompositeDisposable _disposables;

    public override void Construct(
      IPersistentProgressService progressService,
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(progressService, context, openButton);

    protected override void InjectDependencies()
    {
      base.InjectDependencies();

      _soulsTracker = RootContext.Resolve<ISoulsTrackerService>();
    }

    protected override void Initialize() => _disposables = new CompositeDisposable();

    protected override void SubscribeUpdates()
    {
      _soulsTracker.SoulsRP
        .Subscribe(souls => currencyText.text = souls.ToString())
        .AddTo(_disposables);
    }

    protected override void Cleanup()
    {
      base.Cleanup();
      _disposables.Dispose();
    }
  }
}
