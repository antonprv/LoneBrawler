// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;

namespace Code.Data.SaveData.Buffs
{
  /// <summary>
  /// Сериализуемый снимок состояния одного баффа на момент сохранения.
  /// </summary>
  [System.Serializable]
  public class BuffSaveEntry
  {
    public BuffClassName ClassName;
    public BuffActivationType ActivationType;
    public BuffState State;

    /// <summary>
    /// Оставшееся время действия в секундах.
    /// Актуально только для Duration-баффов.
    /// </summary>
    public float RemainingDuration;
  }
}
