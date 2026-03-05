// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.UI.Services.InventoryService.Interfaces;

using Code.Infrastructure.Services.Input.Interfaces;
using Code.UI.Elements.Inventory.Slots;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Inventory.Windows
{
  public class HotbarView : ZenjexBehaviour
  {
    public Transform slotParent;
    public Canvas canvas;

    [Zenjex] private readonly IInputService _inputService;
    [Zenjex] private readonly IInventoryService _inventoryService;
    [Zenjex] private readonly IInventoryFactory _inventoryFactory;

    private List<InventorySlotView> _hotbarSlotViews;

    protected override void OnAwake()
    {
      base.OnAwake();
      AsyncStart().Forget();
    }

    private async UniTaskVoid AsyncStart()
    {
      await InitializeAsync();
      SubscribeToEvents();
    }

    private void OnDestroy() => UnsubscribeFromEvents();

    private async UniTask InitializeAsync()
    {
      _hotbarSlotViews = await _inventoryFactory
        .CreateHotbarElementAsync(slotParent, canvas);

      RefreshAllSlots();
    }

    private void SubscribeToEvents()
    {
      _inventoryService.OnHotbarSlotChanged += OnSlotChanged;
      _inventoryService.OnHotbarSelectionChanged += OnSelectionChanged;
    }

    private void UnsubscribeFromEvents()
    {
      if (_inventoryService != null)
      {
        _inventoryService.OnHotbarSlotChanged -= OnSlotChanged;
        _inventoryService.OnHotbarSelectionChanged -= OnSelectionChanged;
      }
    }

    private void Update() => HandleHotbarInput();

    private void HandleHotbarInput()
    {
      if (_inputService.ActiveHotbar.Value)
        _inventoryService
          .SelectHotbarSlot(_inputService.ActiveHotbar.Key);
    }

    private void OnSlotChanged(int slotIndex)
    {
      if (slotIndex >= 0 && slotIndex < _hotbarSlotViews.Count)
      {
        _hotbarSlotViews[slotIndex].RefreshViewAsync().Forget();
      }
    }

    private void OnSelectionChanged(int newIndex) => UpdateSelection();

    private void UpdateSelection() =>
      _hotbarSlotViews[_inventoryService.SelectedHotbarIndex].SetSelected();

    private void RefreshAllSlots()
    {
      foreach (var slotView in _hotbarSlotViews)
      {
        slotView.RefreshViewAsync().Forget();
      }
    }
  }
}
