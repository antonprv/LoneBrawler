// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.DragDropService.Interfaces;
using Code.Infrastructure.Services.InventoryService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

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

    [Zenjex] private readonly IInventoryService _inventoryService;
    [Zenjex] private readonly IBuffDataSubservice _buffDataService;
    [Zenjex] private readonly IDragDropService _dragDropService;
    [Zenjex] private readonly IAssetLoader _assetLoader;

    private int _slotIndex;
    private DragSource _dragSource;
    private Canvas _parentCanvas;
    private RectTransform _dragLayer;

    private GameObject _dragIcon;
    private ItemTooltipController _tooltip;

    public void Construct(ItemTooltipController tooltip) => _tooltip = tooltip;

    public async UniTask InitializeAsync(int slotIndex, DragSource dragSource, Canvas parentCanvas, RectTransform dragLayer)
    {
      _slotIndex = slotIndex;
      _dragSource = dragSource;
      _parentCanvas = parentCanvas;
      _dragLayer = dragLayer;

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

    public void SetSelected(bool selected)
    {
      if (background != null)
      {
        background.color = selected ? selectedColor : normalColor;
      }
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

      // Determine drag amount based on input
      int dragAmount = CalculateDragAmount(slot, eventData);

      if (dragAmount <= 0)
        return;

      // Start drag
      _dragDropService.StartDrag(slot.BuffClass, dragAmount, _dragSource, _slotIndex);

      // Update source slot
      slot.Count -= dragAmount;
      if (slot.Count <= 0)
      {
        slot.Clear();
      }

      await RefreshViewAsync();

      // Create drag icon
      await CreateDragIconAsync(eventData);
    }

    private int CalculateDragAmount(InventorySlotData slot, PointerEventData eventData)
    {
      // Right click = half
      if (eventData.button == PointerEventData.InputButton.Right)
      {
        return Mathf.CeilToInt(slot.Count / 2f);
      }

      // Left Shift = one
      if (Input.GetKey(KeyCode.LeftShift))
      {
        return 1;
      }

      // Default = all
      return slot.Count;
    }

    private async UniTask CreateDragIconAsync(PointerEventData eventData)
    {
      var buffData = await _buffDataService.ForBuffAsync(_dragDropService.DraggedBuffClass);
      if (buffData == null)
        return;

      _dragIcon = new GameObject("DragIcon");
      _dragIcon.transform.SetParent(_dragLayer, false);

      var dragRT = _dragIcon.AddComponent<RectTransform>();
      var dragImg = _dragIcon.AddComponent<Image>();

      dragImg.sprite = await _assetLoader.LoadAsync<Sprite>(buffData.Icon);
      dragImg.raycastTarget = false;
      dragRT.sizeDelta = icon.rectTransform.sizeDelta;

      UpdateDragIconPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (_dragIcon != null)
      {
        UpdateDragIconPosition(eventData);
      }
    }

    private void UpdateDragIconPosition(PointerEventData eventData)
    {
      if (_dragIcon == null || _dragLayer == null)
        return;

      Camera cam = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ?
        null : _parentCanvas.worldCamera;

      RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _dragLayer,
        eventData.position,
        cam,
        out Vector2 localPoint);

      _dragIcon.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (_dragIcon != null)
      {
        Destroy(_dragIcon);
      }

      // If still dragging, return buffs to source
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

      // Try merge
      if (_dragDropService.TryMergeOrSwap(targetSlot, buffData.MaxStack, out int remaining))
      {
        // If something remains, return to source
        if (remaining > 0)
        {
          ReturnRemainingToSource(remaining);
        }
      }
      else
      {
        // Different buffs - swap
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
      RefreshAllViews();
    }

    private void ReturnRemainingToSource(int remaining)
    {
      var sourceSlot = GetSourceSlot();
      if (sourceSlot == null)
        return;

      if (sourceSlot.IsEmpty)
      {
        sourceSlot.Set(_dragDropService.DraggedBuffClass, remaining);
      }
      else if (sourceSlot.BuffClass == _dragDropService.DraggedBuffClass)
      {
        sourceSlot.Count += remaining;
      }
    }

    private InventorySlotData GetSourceSlot()
    {
      return _dragDropService.Source == DragSource.Inventory
        ? _inventoryService.GetInventorySlot(_dragDropService.SourceIndex)
        : _inventoryService.GetHotbarSlot(_dragDropService.SourceIndex);
    }

    private void RefreshAllViews()
    {
      // This will be called through events, but we can trigger it manually
      RefreshViewAsync().Forget();
    }

    #endregion

    #region Click to Use (from Hotbar)

    public void OnPointerClick(PointerEventData eventData)
    {
      // Only handle clicks from hotbar
      if (_dragSource != DragSource.Hotbar)
        return;

      var slot = GetSlotData();
      if (slot == null || slot.IsEmpty)
        return;

      // Double click or specific button to use buff
      if (eventData.clickCount == 2 || Input.GetKey(KeyCode.LeftControl))
      {
        TryUseBuff(slot.BuffClass);
      }
    }

    private void TryUseBuff(BuffClassName buffClass)
    {
      // This will be handled by player's BuffComponent
      // For now just remove from inventory
      _inventoryService.RemoveBuff(buffClass, 1);
    }

    #endregion

    #region Tooltip

    public async void OnPointerEnter(PointerEventData eventData)
    {
      var slot = GetSlotData();
      if (slot == null || slot.IsEmpty)
        return;

      var buffData = await _buffDataService.ForBuffAsync(slot.BuffClass);
      if (buffData == null)
        return;

      _tooltip.Show(buffData, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      _tooltip.Hide();
    }

    #endregion

    private void Update()
    {
      // Update tooltip position
      if (_tooltip != null && _tooltip.IsVisible())
      {
        _tooltip.UpdatePosition(Input.mousePosition);
      }
    }
  }
}
