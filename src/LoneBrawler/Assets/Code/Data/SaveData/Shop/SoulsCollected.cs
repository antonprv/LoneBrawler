// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

namespace Code.Data.SaveData
{
  [Serializable]
  public class SoulsCollected
  {
    public int Amount;

    public List<string> ClearedSpawners;

    public SoulsCollected()
    {
      Amount = 0;
      ClearedSpawners = new List<string>();
    }
  }
}
