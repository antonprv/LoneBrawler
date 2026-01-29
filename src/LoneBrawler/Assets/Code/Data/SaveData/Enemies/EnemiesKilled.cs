// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

namespace Code.Data.SaveData.Enemies
{
  [Serializable]
  public class EnemiesKilled
  {
    public List<string> ClearedSpawners;

    public EnemiesKilled()
    {
      ClearedSpawners = new List<string>();
    }
  }
}
