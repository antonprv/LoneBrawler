// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Infrastructure.Services.DragDropService.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.InventoryService.Interfaces;
using Code.UI.Elements.Inventory.Slots;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Inventory.Windows
{
  public class HotbarView : ZenjexBehaviour
  {
    public Transform slotParent;
    public GameObject slotPrefab;
    public RectTransform dragLayer;
    public Canvas canvas;
    public ItemTooltipController tooltip;

    [Zenjex] private readonly IInputService _inputService;
    [Zenjex] private readonly IInventoryService _inventoryService;

    private List<InventorySlotView> _slotViews;

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
      _slotViews = new List<InventorySlotView>();

      // Clear existing slots
      foreach (Transform child in slotParent)
        Destroy(child.gameObject);

      // Create slots
      for (int i = 0; i < _inventoryService.HotbarSize; i++)
      {
        var slotGO = Instantiate(slotPrefab, slotParent);
        var slotView = slotGO.GetComponent<InventorySlotView>();

        slotView.Construct(tooltip);
        await slotView.InitializeAsync(i, DragSource.Hotbar, canvas, dragLayer);

        _slotViews.Add(slotView);
      }

      RefreshAllSlots();
      UpdateSelection();
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
      if (slotIndex >= 0 && slotIndex < _slotViews.Count)
      {
        _slotViews[slotIndex].RefreshViewAsync().Forget();
      }
    }

    private void OnSelectionChanged(int newIndex) => UpdateSelection();

    private void UpdateSelection()
    {
      for (int i = 0; i < _slotViews.Count; i++)
      {
        _slotViews[i].SetSelected(i == _inventoryService.SelectedHotbarIndex);
      }
    }

    private void RefreshAllSlots()
    {
      foreach (var slotView in _slotViews)
      {
        slotView.RefreshViewAsync().Forget();
      }
    }
  }
}
