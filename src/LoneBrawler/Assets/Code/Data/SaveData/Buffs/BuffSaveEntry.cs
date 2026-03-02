// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;

namespace Code.Data.SaveData.Buffs
{
  /// <summary>
  /// Serializable snapshot of one buff's state at the moment of saving.
  /// </summary>
  [System.Serializable]
  public class BuffSaveEntry
  {
    public BuffClassName ClassName;
    public BuffActivationType ActivationType;
    public BuffState State;

    /// <summary>
    /// Remaining duration in seconds.
    /// Relevant only for Duration-buffs.
    /// </summary>
    public float RemainingDuration;
  }
}
