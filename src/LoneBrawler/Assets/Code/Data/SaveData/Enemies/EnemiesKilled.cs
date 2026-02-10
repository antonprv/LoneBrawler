// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.CustomTypes.Types;

namespace Code.Data.SaveData.Enemies
{
  [Serializable]
  public class EnemiesKilled
  {
    public HashSetData<string> ClearedSpawners;

    public EnemiesKilled()
    {
      ClearedSpawners = new HashSetData<string>();
    }
  }
}
