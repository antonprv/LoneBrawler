// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DragDropService.Types;

using Code.Gameplay.Utils.ActorComponents;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.TooltipService.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.UI.Elements.Inventory
{
  public class InventorySlotSpawner : MonoBehaviour
  {
    public GameObject containerParent;
    public Canvas parentCanvas;
    public RectTransform dragLayer;
    public CanvasGroup canvasGroup;

    public float showSpeed = 1f;

    private IInventoryFactory _inventoryFactory;
    private IInventoryConfigSubservice _inventoryConfig;
    private ITimeService _timeService;

    private int _slotAmount;

    public void Construct()
    {
      InjectDependencies();
      SetupInternalValues();
      HideCanvas();
    }

    private void InjectDependencies()
    {
      _inventoryFactory = RootContext.Resolve<IInventoryFactory>();
      _inventoryConfig = RootContext.Resolve<IInventoryConfigSubservice>();
      _timeService = RootContext.Resolve<ITimeService>();
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
          parentCanvas,
          dragLayer
        ).Forget();
      }
    }
  }
}
