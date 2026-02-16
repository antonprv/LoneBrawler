// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Common.CustomTypes.Domain.Collections
{
  /// <summary>
  /// Simple key-value pair for serialization
  /// </summary>
  [System.Serializable]
  public class PairData<TKey, TValue>
  {
    public TKey Key;
    public TValue Value;

    public PairData() { }

    public PairData(TKey key, TValue value)
    {
      Key = key;
      Value = value;
    }

    public bool IsValid() => this != null;

    public override string ToString() => $"({Key}, {Value})";
  }
}
