// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData.Types.Buff;

namespace Code.Data.SaveData.Inventory
{
  [Serializable]
  public class InventorySlotData
  {
    public BuffClassName BuffClass;
    public int Count;

    public InventorySlotData()
    {
      BuffClass = BuffClassName.None;
      Count = 0;
    }

    public InventorySlotData(BuffClassName buffClass, int count)
    {
      BuffClass = buffClass;
      Count = count;
    }

    public bool IsEmpty => BuffClass == BuffClassName.None || Count <= 0;

    public void Clear()
    {
      BuffClass = BuffClassName.None;
      Count = 0;
    }

    public void Set(BuffClassName buffClass, int count)
    {
      BuffClass = buffClass;
      Count = count;
    }
  }
}
