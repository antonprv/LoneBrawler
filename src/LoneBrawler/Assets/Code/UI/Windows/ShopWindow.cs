// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.UI.Elements.Shop;
using Code.UI.Windows.Types;

using R3;

using TMPro;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;

namespace Code.UI.Windows
{
  public class ShopWindow : WindowBase
  {
    public TextMeshProUGUI currencyText;
    public ShopItemSpawner itemSpawner;

    [Zenjex] private readonly ISoulsTrackerService _soulsTracker;

    private CompositeDisposable _disposables = new();

    public override void Construct(
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(context, openButton);

    protected override void SetWindowType() =>
      windowTypeId = WindowTypeId.Shop;

    protected override void Initialize()
    {
      base.Initialize();

      itemSpawner.Construct();
      _disposables = new CompositeDisposable();
    }

    protected override void SubscribeUpdates()
    {
      _soulsTracker.SoulsRP
        .Subscribe(souls => currencyText.text = souls.ToString())
        .AddTo(_disposables);

      itemSpawner.SpawnItems();
    }

    protected override void Cleanup()
    {
      base.Cleanup();
      _disposables.Dispose();
    }
  }
}
