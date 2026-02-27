// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;

namespace Code.Infrastructure.Services.DragDropService.Interfaces
{
  public enum DragSource
  {
    None,
    Inventory,
    Hotbar
  }

  public interface IDragDropService
  {
    event Action OnDragStarted;
    event Action OnDragEnded;

    bool IsDragging { get; }
    BuffClassName DraggedBuffClass { get; }
    int DraggedCount { get; }
    DragSource Source { get; }
    int SourceIndex { get; }

    void StartDrag(BuffClassName buffClass, int count, DragSource source, int sourceIndex);
    void EndDrag();
    void CancelDrag();
    
    bool TryMergeOrSwap(InventorySlotData targetSlot, int maxStack, out int remainingCount);
  }
}
