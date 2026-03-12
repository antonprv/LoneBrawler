// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.DragDropService.Interfaces;
using Code.UI.Services.DragDropService.Types;
using Code.UI.Services.InventoryService.Interfaces;

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Player.Buffs.Interfaces;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.UI.Services.DragIcon.Interfaces;
using Code.UI.Services.TooltipService.Interfaces;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Zenjex.Extensions.Core;
using Code.Infrastructure.Services.Time;
using System;
using Zenjex.Extensions.Injector;
using Zenjex.Extensions.Attribute;

namespace Code.UI.Elements.Inventory.Slots
{
  public class InventorySlotView : ZenjexBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
  {
    public Image icon;
    public TextMeshProUGUI countText;
    public Image background;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    public float colorSwitchSpeed = 0.125f;

    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly IInventoryService _inventoryService;
    [Zenjex] private readonly IBuffDataSubservice _buffDataService;
    [Zenjex] private readonly IDragDropService _dragDropService;
    [Zenjex] private readonly IAssetLoader _assetLoader;
    [Zenjex] private readonly ITooltipProvider _tooltipProvider;
    [Zenjex] private readonly IDragIconProvider _dragIconProvider;

    private int _slotIndex;
    private DragSource _dragSource;

    public async UniTask InitializeAsync(
      int slotIndex,
      DragSource dragSource
      )
    {
      _slotIndex = slotIndex;
      _dragSource = dragSource;

      await RefreshViewAsync();
    }

    public async UniTask RefreshViewAsync()
    {
      var slot = GetSlotData();
      if (slot == null || slot.IsEmpty)
      {
        SetEmpty();
        return;
      }

      var buffData = await _buffDataService.ForBuffAsync(slot.BuffClass);
      if (buffData == null)
      {
        SetEmpty();
        return;
      }

      icon.enabled = true;
      icon.sprite = await _assetLoader.LoadAsync<Sprite>(buffData.Icon);
      countText.text = slot.Count > 1 ? slot.Count.ToString() : "";
    }

    public void SetSelected()
    {
      if (background == null) return;

      SwitchColor(background, selectedColor);

      var slot = GetSlotData();

      if (slot == null || slot.IsEmpty) return;
      TryUseBuff(slot.BuffClass);
    }

    private void SwitchColor(Image image, Color targetColor)
    {
      LeanTween
        .color(
          image.rectTransform,
          targetColor,
          colorSwitchSpeed * _timeService.DeltaAt100FPS)
        .setEaseInOutCubic()
        .setOnComplete(() =>
        {
          LeanTween
          .color(
            image.rectTransform,
            normalColor,
            colorSwitchSpeed * _timeService.DeltaAt100FPS)
          .setEaseInOutCubic();
        });
    }

    private void SetEmpty()
    {
      icon.enabled = false;
      countText.text = "";
    }

    private InventorySlotData GetSlotData()
    {
      return _dragSource == DragSource.Inventory
        ? _inventoryService.GetInventorySlot(_slotIndex)
        : _inventoryService.GetHotbarSlot(_slotIndex);
    }

    #region Drag & Drop

    public async void OnBeginDrag(PointerEventData eventData)
    {
      var slot = GetSlotData();
      if (slot == null || slot.IsEmpty)
        return;

      int dragAmount = CalculateDragAmount(slot, eventData);
      if (dragAmount <= 0)
        return;

      _dragDropService.StartDrag(slot.BuffClass, dragAmount, _dragSource, _slotIndex);

      slot.Count -= dragAmount;
      if (slot.Count <= 0)
        slot.Clear();

      RefreshViewAsync().Forget();

      // Show icon immediately - use already loaded slot sprite
      var dragIcon = _dragIconProvider.GetDragIcon();
      if (dragIcon != null)
        dragIcon.Show(icon.sprite, eventData.position);

      // Load sprite additionally in case it differs (e.g., different size)
      var buffData = await _buffDataService.ForBuffAsync(_dragDropService.DraggedBuffClass);
      if (buffData != null && dragIcon != null && dragIcon.gameObject.activeSelf)
      {
        var sprite = await _assetLoader.LoadAsync<Sprite>(buffData.Icon);
        dragIcon.icon.sprite = sprite;
      }
    }

    private int CalculateDragAmount(InventorySlotData slot, PointerEventData eventData)
    {
      if (eventData.button == PointerEventData.InputButton.Right)
        return Mathf.CeilToInt(slot.Count / 2f);

      if (Keyboard.current.leftShiftKey.isPressed)
        return 1;

      return slot.Count;
    }

    public void OnDrag(PointerEventData eventData)
    {
      var dragIcon = _dragIconProvider.GetDragIcon();
      if (dragIcon != null)
        dragIcon.UpdatePosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      var dragIcon = _dragIconProvider.GetDragIcon();
      if (dragIcon != null)
        dragIcon.Hide();

      // If drag ended not on a slot - return items back
      if (_dragDropService.IsDragging)
      {
        var slot = GetSlotData();
        if (slot.IsEmpty)
        {
          slot.Set(_dragDropService.DraggedBuffClass, _dragDropService.DraggedCount);
        }
        else if (slot.BuffClass == _dragDropService.DraggedBuffClass)
        {
          slot.Count += _dragDropService.DraggedCount;
        }

        _dragDropService.EndDrag();
        RefreshViewAsync().Forget();
      }


    }

    public async void OnDrop(PointerEventData eventData)
    {
      if (!_dragDropService.IsDragging)
        return;

      var targetSlot = GetSlotData();
      var buffData = await _buffDataService.ForBuffAsync(_dragDropService.DraggedBuffClass);

      if (buffData == null)
      {
        _dragDropService.EndDrag();
        return;
      }

      if (_dragDropService.TryMergeOrSwap(targetSlot, buffData.MaxStack, out int remaining))
      {
        if (remaining > 0)
          ReturnRemainingToSource(remaining);
      }
      else
      {
        // Different items - swap
        var sourceSlot = GetSourceSlot();
        if (sourceSlot != null)
        {
          BuffClassName tempBuffClass = targetSlot.BuffClass;
          int tempCount = targetSlot.Count;

          targetSlot.Set(_dragDropService.DraggedBuffClass, _dragDropService.DraggedCount);
          sourceSlot.Set(tempBuffClass, tempCount);
        }
      }

      _dragDropService.EndDrag();
      RefreshViewAsync().Forget();
    }

    private void ReturnRemainingToSource(int remaining)
    {
      var sourceSlot = GetSourceSlot();
      if (sourceSlot == null)
        return;

      if (sourceSlot.IsEmpty)
        sourceSlot.Set(_dragDropService.DraggedBuffClass, remaining);
      else if (sourceSlot.BuffClass == _dragDropService.DraggedBuffClass)
        sourceSlot.Count += remaining;
    }

    private InventorySlotData GetSourceSlot()
    {
      return _dragDropService.Source == DragSource.Inventory
        ? _inventoryService.GetInventorySlot(_dragDropService.SourceIndex)
        : _inventoryService.GetHotbarSlot(_dragDropService.SourceIndex);
    }

    #endregion

    #region Click to Use (from Hotbar)

    public void OnPointerClick(PointerEventData eventData)
    {
      if (_dragSource != DragSource.Hotbar)
        return;

      var slot = GetSlotData();
      if (slot == null || slot.IsEmpty)
        return;

      if (eventData.clickCount == 2 || Keyboard.current.leftCtrlKey.isPressed)
        TryUseBuff(slot.BuffClass);
    }

    private void TryUseBuff(BuffClassName buffClass)
    {
      var consumer = RootContext.Resolve<IBuffConsumer>();
      if (consumer == null) return;
      consumer.ConsumeBuff(buffClass);
      _inventoryService.RemoveBuff(buffClass, 1);
    }

    #endregion

    #region Tooltip

    private bool _isPointerOver;

    public async void OnPointerEnter(PointerEventData eventData)
    {
      _isPointerOver = true;

      var tooltip = _tooltipProvider.GetTooltip();

      var slot = GetSlotData();
      if (slot == null || slot.IsEmpty)
        return;

      var buffData = await _buffDataService.ForBuffAsync(slot.BuffClass);

      if (!_isPointerOver || buffData == null)
        return;

      if (tooltip != null)
        tooltip.Show(buffData, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      _isPointerOver = false;

      var tooltip = _tooltipProvider.GetTooltip();
      if (tooltip != null)
        tooltip.Hide();
    }

    #endregion

    private void Update()
    {
      var tooltip = _tooltipProvider.GetTooltip();

      if (tooltip != null && tooltip.IsVisible())
        tooltip.UpdatePosition(Mouse.current.position.ReadValue());
    }
  }
}
