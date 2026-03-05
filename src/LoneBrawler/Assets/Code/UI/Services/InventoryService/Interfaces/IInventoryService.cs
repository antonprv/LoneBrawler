// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;

using Cysharp.Threading.Tasks;

namespace Code.UI.Services.InventoryService.Interfaces
{
  public interface IInventoryService
  {
    public event Action<int> OnInventorySlotChanged;
    public event Action<int> OnHotbarSlotChanged;
    public event Action<int> OnHotbarSelectionChanged;

    public int InventorySize { get; }
    public int HotbarSize { get; }
    public int SelectedHotbarIndex { get; }

    public void Initialize(int inventorySize, int hotbarSize);

    // Inventory Operations
    public UniTask<bool> AddBuffAsync(BuffClassName buffClass, int count = 1, bool tryHotbarFirst = false);
    public bool RemoveBuff(BuffClassName buffClass, int count = 1);
    public bool HasBuff(BuffClassName buffClass, int minCount = 1);
    public int GetBuffCount(BuffClassName buffClass);

    // Slot Operations
    public InventorySlotData GetInventorySlot(int index);
    public InventorySlotData GetHotbarSlot(int index);
    public bool MoveOrSwapInventorySlots(int fromIndex, int toIndex);
    public bool MoveOrSwapHotbarSlots(int fromIndex, int toIndex);
    public bool MoveInventoryToHotbar(int inventoryIndex, int hotbarIndex);
    public bool MoveHotbarToInventory(int hotbarIndex, int inventoryIndex);

    // Hotbar Operations
    public void SelectHotbarSlot(int index);
    public UniTask<BuffStaticData> GetSelectedHotbarBuffAsync();

    // Query
    public List<InventorySlotData> GetAllInventorySlots();
    public List<InventorySlotData> GetAllHotbarSlots();

    // Save/Load
    public InventorySaveData GetSaveData();
    public void LoadFromSaveData(InventorySaveData saveData);

    // Clear
    public void ClearInventory();
    public void ClearHotbar();
    public void ClearAll();
  }
}
