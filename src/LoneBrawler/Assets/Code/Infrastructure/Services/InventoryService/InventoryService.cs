// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Common.Extensions.Logging;
using Code.Data.SaveData.Inventory;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.Services.InventoryService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Services.InventoryService
{
  public class InventoryService : IInventoryService
  {
    public event Action<int> OnInventorySlotChanged;
    public event Action<int> OnHotbarSlotChanged;
    public event Action<int> OnHotbarSelectionChanged;

    public int InventorySize { get; private set; }
    public int HotbarSize { get; private set; }
    public int SelectedHotbarIndex { get; private set; }

    private readonly IBuffDataSubservice _buffDataService;
    private readonly IGameLog _logger;

    private List<InventorySlotData> _inventorySlots;
    private List<InventorySlotData> _hotbarSlots;

    public InventoryService(IBuffDataSubservice buffDataService, IGameLog logger)
    {
      _buffDataService = buffDataService;
      _logger = logger;
    }

    public void Initialize(int inventorySize, int hotbarSize)
    {
      InventorySize = inventorySize;
      HotbarSize = hotbarSize;
      SelectedHotbarIndex = 0;

      _inventorySlots = new List<InventorySlotData>(inventorySize);
      _hotbarSlots = new List<InventorySlotData>(hotbarSize);

      for (int i = 0; i < inventorySize; i++)
      {
        _inventorySlots.Add(new InventorySlotData());
      }

      for (int i = 0; i < hotbarSize; i++)
      {
        _hotbarSlots.Add(new InventorySlotData());
      }

      _logger.Log(
        $"Initialized with {inventorySize} inventory slots and {hotbarSize} hotbar slots");
    }

    #region Add/Remove Buffs

    public async UniTask<bool> AddBuffAsync(BuffClassName buffClass, int count = 1, bool tryHotbarFirst = false)
    {
      if (buffClass == BuffClassName.None)
      {
        _logger.Log(LogType.Warning, "Cannot add None buff");
        return false;
      }

      var buffData = await _buffDataService.ForBuffAsync(buffClass);
      if (buffData == null)
      {
        _logger.Log(LogType.Error,
          $"[InventoryService] Buff data not found for {buffClass}");
        return false;
      }

      if (count <= 0)
      {
        _logger.Log(LogType.Warning,
          "[InventoryService] Cannot add buff with count <= 0");
        return false;
      }

      int remaining = count;

      // Try hotbar first if requested
      if (tryHotbarFirst)
      {
        remaining = TryAddToSlots(_hotbarSlots, buffClass, remaining, buffData.MaxStack, OnHotbarSlotChanged);
        if (remaining <= 0)
          return true;
      }

      // Try inventory
      remaining = TryAddToSlots(_inventorySlots, buffClass, remaining, buffData.MaxStack, OnInventorySlotChanged);

      // If still remaining and didn't try hotbar first, try it now
      if (remaining > 0 && !tryHotbarFirst)
      {
        remaining = TryAddToSlots(_hotbarSlots, buffClass, remaining, buffData.MaxStack, OnHotbarSlotChanged);
      }

      if (remaining > 0)
      {
        _logger.Log(LogType.Warning,
          $"[InventoryService] " +
          $"Added {count - remaining} " +
          $"of {count} buffs (inventory full)");
        return false;
      }

      _logger.Log($"[InventoryService] Added {count}x {buffData.DisplayName}");
      return true;
    }

    private int TryAddToSlots(List<InventorySlotData> slots, BuffClassName buffClass, 
      int count, int maxStack, Action<int> onSlotChanged)
    {
      int remaining = count;

      // First pass: try to stack with existing buffs
      for (int i = 0; i < slots.Count; i++)
      {
        var slot = slots[i];
        if (!slot.IsEmpty && slot.BuffClass == buffClass && slot.Count < maxStack)
        {
          int space = maxStack - slot.Count;
          int toAdd = Mathf.Min(space, remaining);

          slot.Count += toAdd;
          remaining -= toAdd;

          onSlotChanged?.Invoke(i);

          if (remaining <= 0)
            return 0;
        }
      }

      // Second pass: add to empty slots
      for (int i = 0; i < slots.Count; i++)
      {
        var slot = slots[i];
        if (slot.IsEmpty)
        {
          int toAdd = Mathf.Min(maxStack, remaining);
          slot.Set(buffClass, toAdd);
          remaining -= toAdd;

          onSlotChanged?.Invoke(i);

          if (remaining <= 0)
            return 0;
        }
      }

      return remaining;
    }

    public bool RemoveBuff(BuffClassName buffClass, int count = 1)
    {
      if (!HasBuff(buffClass, count))
      {
        _logger.Log(LogType.Warning,
          $"Cannot remove {count}x {buffClass}: not enough in inventory");
        return false;
      }

      int remaining = count;

      // Remove from inventory
      remaining = RemoveFromSlots(_inventorySlots, buffClass, remaining, OnInventorySlotChanged);

      // Remove from hotbar if needed
      if (remaining > 0)
      {
        _ = RemoveFromSlots(_hotbarSlots, buffClass, remaining, OnHotbarSlotChanged);
      }

      _logger.Log($"Removed {count}x {buffClass}");
      return true;
    }

    private int RemoveFromSlots(List<InventorySlotData> slots, BuffClassName buffClass, 
      int count, Action<int> onSlotChanged)
    {
      int remaining = count;

      for (int i = 0; i < slots.Count; i++)
      {
        var slot = slots[i];
        if (!slot.IsEmpty && slot.BuffClass == buffClass)
        {
          int toRemove = Mathf.Min(slot.Count, remaining);
          slot.Count -= toRemove;
          remaining -= toRemove;

          if (slot.Count <= 0)
          {
            slot.Clear();
          }

          onSlotChanged?.Invoke(i);

          if (remaining <= 0)
            return 0;
        }
      }

      return remaining;
    }

    public bool HasBuff(BuffClassName buffClass, int minCount = 1)
    {
      return GetBuffCount(buffClass) >= minCount;
    }

    public int GetBuffCount(BuffClassName buffClass)
    {
      int count = 0;

      foreach (var slot in _inventorySlots)
      {
        if (!slot.IsEmpty && slot.BuffClass == buffClass)
        {
          count += slot.Count;
        }
      }

      foreach (var slot in _hotbarSlots)
      {
        if (!slot.IsEmpty && slot.BuffClass == buffClass)
        {
          count += slot.Count;
        }
      }

      return count;
    }

    #endregion

    #region Slot Operations

    public InventorySlotData GetInventorySlot(int index)
    {
      if (index < 0 || index >= InventorySize)
      {
        _logger.Log(LogType.Error, $"Invalid inventory slot index: {index}");
        return null;
      }

      return _inventorySlots[index];
    }

    public InventorySlotData GetHotbarSlot(int index)
    {
      if (index < 0 || index >= HotbarSize)
      {
        _logger.Log(LogType.Error, $"Invalid hotbar slot index: {index}");
        return null;
      }

      return _hotbarSlots[index];
    }

    public bool MoveOrSwapInventorySlots(int fromIndex, int toIndex)
    {
      if (!ValidateSlotIndices(fromIndex, toIndex, InventorySize))
        return false;

      SwapSlots(_inventorySlots[fromIndex], _inventorySlots[toIndex]);

      OnInventorySlotChanged?.Invoke(fromIndex);
      OnInventorySlotChanged?.Invoke(toIndex);

      return true;
    }

    public bool MoveOrSwapHotbarSlots(int fromIndex, int toIndex)
    {
      if (!ValidateSlotIndices(fromIndex, toIndex, HotbarSize))
        return false;

      SwapSlots(_hotbarSlots[fromIndex], _hotbarSlots[toIndex]);

      OnHotbarSlotChanged?.Invoke(fromIndex);
      OnHotbarSlotChanged?.Invoke(toIndex);

      return true;
    }

    public bool MoveInventoryToHotbar(int inventoryIndex, int hotbarIndex)
    {
      if (inventoryIndex < 0 || inventoryIndex >= InventorySize ||
          hotbarIndex < 0 || hotbarIndex >= HotbarSize)
      {
        _logger.Log(LogType.Error,
          $"Invalid indices for move: inventory={inventoryIndex}, hotbar={hotbarIndex}");
        return false;
      }

      SwapSlots(_inventorySlots[inventoryIndex], _hotbarSlots[hotbarIndex]);

      OnInventorySlotChanged?.Invoke(inventoryIndex);
      OnHotbarSlotChanged?.Invoke(hotbarIndex);

      return true;
    }

    public bool MoveHotbarToInventory(int hotbarIndex, int inventoryIndex)
    {
      return MoveInventoryToHotbar(inventoryIndex, hotbarIndex);
    }

    private bool ValidateSlotIndices(int from, int to, int maxSize)
    {
      if (from == to)
        return false;

      if (from < 0 || from >= maxSize || to < 0 || to >= maxSize)
      {
        _logger.Log(LogType.Error, $"Invalid slot indices: from={from}, to={to}, max={maxSize}");
        return false;
      }

      return true;
    }

    private void SwapSlots(InventorySlotData from, InventorySlotData to)
    {
      BuffClassName tempBuffClass = from.BuffClass;
      int tempCount = from.Count;

      from.Set(to.BuffClass, to.Count);
      to.Set(tempBuffClass, tempCount);
    }

    #endregion

    #region Hotbar Operations

    public void SelectHotbarSlot(int index)
    {
      if (index < 0 || index >= HotbarSize)
      {
        _logger.Log(LogType.Warning, $"Invalid hotbar selection index: {index}");
        return;
      }

      if (SelectedHotbarIndex == index)
        return;

      SelectedHotbarIndex = index;
      OnHotbarSelectionChanged?.Invoke(index);

      _logger.Log($"Selected hotbar slot {index}");
    }

    public async UniTask<BuffStaticData> GetSelectedHotbarBuffAsync()
    {
      var slot = GetHotbarSlot(SelectedHotbarIndex);
      if (slot == null || slot.IsEmpty)
        return null;

      return await _buffDataService.ForBuffAsync(slot.BuffClass);
    }

    #endregion

    #region Query

    public List<InventorySlotData> GetAllInventorySlots()
    {
      return new List<InventorySlotData>(_inventorySlots);
    }

    public List<InventorySlotData> GetAllHotbarSlots()
    {
      return new List<InventorySlotData>(_hotbarSlots);
    }

    #endregion

    #region Save/Load

    public InventorySaveData GetSaveData()
    {
      var saveData = new InventorySaveData
      {
        InventorySlots = new List<InventorySlotData>(_inventorySlots),
        HotbarSlots = new List<InventorySlotData>(_hotbarSlots),
        SelectedHotbarIndex = SelectedHotbarIndex
      };

      return saveData;
    }

    public void LoadFromSaveData(InventorySaveData saveData)
    {
      if (saveData == null)
      {
        _logger.Log(LogType.Error, "Cannot load from null save data");
        return;
      }

      // Initialize if not already done
      if (_inventorySlots == null || _hotbarSlots == null)
      {
        Initialize(saveData.InventorySlots.Count, saveData.HotbarSlots.Count);
      }

      // Load inventory
      for (int i = 0; i < Mathf.Min(saveData.InventorySlots.Count, InventorySize); i++)
      {
        _inventorySlots[i].Set(saveData.InventorySlots[i].BuffClass, saveData.InventorySlots[i].Count);
        OnInventorySlotChanged?.Invoke(i);
      }

      // Load hotbar
      for (int i = 0; i < Mathf.Min(saveData.HotbarSlots.Count, HotbarSize); i++)
      {
        _hotbarSlots[i].Set(saveData.HotbarSlots[i].BuffClass, saveData.HotbarSlots[i].Count);
        OnHotbarSlotChanged?.Invoke(i);
      }

      SelectedHotbarIndex = saveData.SelectedHotbarIndex;
      OnHotbarSelectionChanged?.Invoke(SelectedHotbarIndex);

      _logger.Log("Loaded from save data");
    }

    #endregion

    #region Clear

    public void ClearInventory()
    {
      for (int i = 0; i < _inventorySlots.Count; i++)
      {
        _inventorySlots[i].Clear();
        OnInventorySlotChanged?.Invoke(i);
      }

      _logger.Log("Cleared inventory");
    }

    public void ClearHotbar()
    {
      for (int i = 0; i < _hotbarSlots.Count; i++)
      {
        _hotbarSlots[i].Clear();
        OnHotbarSlotChanged?.Invoke(i);
      }

      _logger.Log("Cleared hotbar");
    }

    public void ClearAll()
    {
      ClearInventory();
      ClearHotbar();
      SelectedHotbarIndex = 0;
      OnHotbarSelectionChanged?.Invoke(0);
    }

    #endregion
  }
}
