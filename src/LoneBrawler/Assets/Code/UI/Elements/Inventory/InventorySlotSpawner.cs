// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.DragDropService.Types;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Core;
using Zenjex.Extensions.Injector;
using Zenjex.Extensions.Attribute;

namespace Code.UI.Elements.Inventory
{
  public class InventorySlotSpawner : ZenjexBehaviour
  {
    public GameObject containerParent;
    public Canvas parentCanvas;
    public CanvasGroup canvasGroup;

    public float showSpeed = 1.15f;

    [Zenjex] private readonly IInventoryFactory _inventoryFactory;
    [Zenjex] private readonly IInventoryConfigSubservice _inventoryConfig;
    [Zenjex] private readonly ITimeService _timeService;

    private int _slotAmount;

    public void Construct()
    {
      SetupInternalValues();
      HideCanvas();
    }

    public void CreateInventory()
    {
      SpawnSlots();
      SmoothShowCanvas();
    }

    private void SmoothShowCanvas() => LeanTween
        .alphaCanvas(canvasGroup, 1f, showSpeed * _timeService.DeltaAt100FPS)
        .setEase(LeanTweenType.easeOutCubic);
    private void HideCanvas() => canvasGroup.alpha = 0;

    private void SetupInternalValues() => _slotAmount = _inventoryConfig.InventorySize;

    private void SpawnSlots()
    {
      for (int slotIndex = 0; slotIndex < _slotAmount; slotIndex++)
      {
        _inventoryFactory.CreateInventorySlotAsync(
          containerParent.transform,
          slotIndex,
          DragSource.Inventory,
          parentCanvas
        ).Forget();
      }
    }
  }
}
