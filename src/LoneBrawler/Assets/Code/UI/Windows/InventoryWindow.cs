// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.UI.Elements.Inventory;
using Code.UI.Services.TooltipService.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine.UI;

using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class InventoryWindow : WindowBase
  {
    public ItemTooltipController tooltipController;
    public InventorySlotSpawner inventorySlotSpawner;

    private ITooltipReceiver _tooltipReceiver;

    public override WindowTypeId WindowType => WindowTypeId.Inventory;

    public override void Construct(
      IPersistentProgressService progressService,
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(progressService, context, openButton);

    protected override void InjectDependencies() =>
      _tooltipReceiver = RootContext.Resolve<ITooltipReceiver>();

    protected override void Initialize()
    {
      base.Initialize();

      tooltipController.Construct();
      _tooltipReceiver.SetTooltip(tooltipController);

      inventorySlotSpawner.Construct();
      inventorySlotSpawner.CreateInventory();
    }

    //private void OnAdditionalButtonClicked() => Destroy(gameObject);

    protected override void SubscribeUpdates()
    {
      base.SubscribeUpdates();

      //_openButton.onClick.AddListener(OnAdditionalButtonClicked);
    }

    protected override void Cleanup()
    {
      //if (_openButton != null)
        //_openButton.onClick.RemoveListener(OnAdditionalButtonClicked);

      base.Cleanup();
    }
  }
}
