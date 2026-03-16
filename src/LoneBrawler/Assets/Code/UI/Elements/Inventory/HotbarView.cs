// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Infrastructure.Services.Input.Interfaces;
using Code.UI.Elements.Inventory.Slots;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.InventoryService.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

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
    private CompositeDisposable _disposables = new();

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

    private void OnDestroy() => _disposables?.Dispose();

    private async UniTask InitializeAsync()
    {
      _hotbarSlotViews = await _inventoryFactory
        .CreateHotbarElementsAsync(slotParent, canvas);

      RefreshAllSlots();
    }

    private void SubscribeToEvents()
    {
      _disposables = new CompositeDisposable();

      _inventoryService.OnHotbarSlotChanged
        .Subscribe(OnSlotChanged)
        .AddTo(_disposables);

      _inventoryService.OnHotbarSelectionChanged
        .Subscribe(_ => UpdateSelection())
        .AddTo(_disposables);
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
        _hotbarSlotViews[slotIndex].RefreshViewAsync().Forget();
    }

    private void UpdateSelection() =>
      _hotbarSlotViews[_inventoryService.SelectedHotbarIndex].SetSelected();

    private void RefreshAllSlots()
    {
      foreach (var slotView in _hotbarSlotViews)
        slotView.RefreshViewAsync().Forget();
    }
  }
}
