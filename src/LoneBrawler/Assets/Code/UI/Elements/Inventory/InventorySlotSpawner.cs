// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.UI.Factory.Interfaces;

using Code.Common.FastMath;
using Code.Infrastructure.Services.DragDropService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Inventory
{
  public class InventorySlotSpawner : ZenjexBehaviour
  {
    public ItemTooltipController controller;
    public Canvas parentCanvas;
    public RectTransform dragSource;
    public CanvasGroup canvasGroup;

    [Zenjex] private readonly IInventoryFactory _inventorySlotFactory;
    [Zenjex] private readonly IInventoryConfigSubservice _inventoryConfig;
    [Zenjex] private readonly IDragDropService _dragDropService;
    [Zenjex] private readonly ITimeService _timeService;

    private int _slotAmount;

    protected override async void OnAwake()
    {
      base.OnAwake();

      SetupInternalValues();

      HideCanvas();

      await SpawnSlots();

      StartCoroutine(FadeOut());
    }

    private void HideCanvas() => canvasGroup.alpha = 0;

    private void SetupInternalValues()
    {
      _slotAmount = _inventoryConfig.InventorySize;
    }

    private async UniTask SpawnSlots()
    {
      for (int slotIndex = 0; slotIndex < _slotAmount; slotIndex++)
      {
        await _inventorySlotFactory.CreateInventorySlotAsync(
          controller,
          slotIndex,
          _dragDropService.Source,
          parentCanvas,
          dragSource
          );

        await UniTask.Yield();
      }
    }

    private IEnumerator FadeOut()
    {
      while (!canvasGroup.alpha.IsNearlyEqual(1))
      {
        canvasGroup.alpha += _timeService.UnscaledDeltaTime;
        yield return null;
      }
    }
  }
}
