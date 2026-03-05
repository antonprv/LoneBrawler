// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.UI.Services.DragDropService.Interfaces;
using Code.UI.Services.DragDropService.Types;

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;

using UnityEngine;

namespace Code.UI.Services.DragDropService
{
  public class DragDropService : IDragDropService
  {
    public event Action OnDragStarted;
    public event Action OnDragEnded;

    public bool IsDragging { get; private set; }
    public BuffClassName DraggedBuffClass { get; private set; }
    public int DraggedCount { get; private set; }
    public DragSource Source { get; private set; }
    public int SourceIndex { get; private set; }

    public void StartDrag(BuffClassName buffClass, int count, DragSource source, int sourceIndex)
    {
      if (buffClass == BuffClassName.None || count <= 0)
      {
        Debug.LogWarning($"[DragDropService] Cannot start drag with invalid data: buff={buffClass}, count={count}");
        return;
      }

      IsDragging = true;
      DraggedBuffClass = buffClass;
      DraggedCount = count;
      Source = source;
      SourceIndex = sourceIndex;

      OnDragStarted?.Invoke();
    }

    public void EndDrag()
    {
      if (!IsDragging)
        return;

      IsDragging = false;
      DraggedBuffClass = BuffClassName.None;
      DraggedCount = 0;
      Source = DragSource.None;
      SourceIndex = -1;

      OnDragEnded?.Invoke();
    }

    public void CancelDrag()
    {
      if (!IsDragging)
        return;

      // When cancelled, the dragged buffs should return to source
      // This is handled by the caller
      EndDrag();
    }

    public bool TryMergeOrSwap(InventorySlotData targetSlot, int maxStack, out int remainingCount)
    {
      remainingCount = DraggedCount;

      if (!IsDragging)
        return false;

      // Empty slot - move all
      if (targetSlot.IsEmpty)
      {
        targetSlot.Set(DraggedBuffClass, DraggedCount);
        remainingCount = 0;
        return true;
      }

      // Same buff - try to merge
      if (targetSlot.BuffClass == DraggedBuffClass)
      {
        int availableSpace = maxStack - targetSlot.Count;
        int amountToAdd = Mathf.Min(availableSpace, DraggedCount);

        targetSlot.Count += amountToAdd;
        remainingCount = DraggedCount - amountToAdd;

        return true;
      }

      // Different buff - swap will be handled by caller
      return false;
    }
  }
}
