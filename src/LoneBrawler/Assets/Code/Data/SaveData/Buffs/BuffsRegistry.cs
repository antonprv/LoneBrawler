// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

namespace Code.Data.SaveData.Buffs
{
  /// <summary>
  /// Saved list of player buffs.
  /// Stores snapshots of states, not live objects.
  /// </summary>
  [System.Serializable]
  public class BuffsRegistry
  {
    public List<BuffSaveEntry> PlayerBuffs;

    public BuffsRegistry() => PlayerBuffs = new();
  }
}
