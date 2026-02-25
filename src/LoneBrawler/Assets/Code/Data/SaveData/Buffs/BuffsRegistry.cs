// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

namespace Code.Data.SaveData.Buffs
{
  /// <summary>
  /// Сохраняемый список баффов игрока.
  /// Хранит снимки состояний, а не живые объекты.
  /// </summary>
  [System.Serializable]
  public class BuffsRegistry
  {
    public List<BuffSaveEntry> PlayerBuffs = new();
  }
}
