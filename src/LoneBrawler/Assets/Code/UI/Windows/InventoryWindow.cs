// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.UI.Elements.Inventory;
using Code.UI.Services.DragIcon.Interfaces;
using Code.UI.Services.TooltipService.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class InventoryWindow : WindowBase
  {
    [Zenjex] private readonly ITooltipReceiver _tooltipReceiver;
    [Zenjex] private readonly IDragIconReceiver _dragIconReceiver;

    public ItemTooltipController tooltipController;
    public InventorySlotSpawner inventorySlotSpawner;
    public DragIconView dragIconView;

    public override void Construct(
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(context, openButton);

    protected override void SetWindowType() =>
      windowTypeId = WindowTypeId.Inventory;

    protected override void Initialize()
    {
      base.Initialize();

      tooltipController.Construct();
      _tooltipReceiver.SetTooltip(tooltipController);

      dragIconView.Construct();
      _dragIconReceiver.SetDragIcon(dragIconView);

      inventorySlotSpawner.Construct();
      inventorySlotSpawner.CreateInventory();
    }

    private void OnAdditionalButtonClicked() => Destroy(gameObject);

    protected override void SubscribeUpdates()
    {
      base.SubscribeUpdates();

      _openButton.onClick.AddListener(OnAdditionalButtonClicked);
      _openButton.OnDeselect(null);
    }

    protected override void Cleanup()
    {
      if (_openButton != null)
        _openButton.onClick.RemoveListener(OnAdditionalButtonClicked);

      base.Cleanup();
    }
  }
}
