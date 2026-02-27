// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

namespace Code.Data.SaveData.Inventory
{
  [Serializable]
  public class InventorySaveData
  {
    public List<InventorySlotData> InventorySlots;
    public List<InventorySlotData> HotbarSlots;
    public int SelectedHotbarIndex;

    public InventorySaveData()
    {
      InventorySlots = new List<InventorySlotData>();
      HotbarSlots = new List<InventorySlotData>();
      SelectedHotbarIndex = 0;
    }

    public void InitializeSlots(int inventorySize, int hotbarSize)
    {
      InventorySlots = new List<InventorySlotData>(inventorySize);
      HotbarSlots = new List<InventorySlotData>(hotbarSize);

      for (int i = 0; i < inventorySize; i++)
      {
        InventorySlots.Add(new InventorySlotData());
      }

      for (int i = 0; i < hotbarSize; i++)
      {
        HotbarSlots.Add(new InventorySlotData());
      }
    }
  }
}
