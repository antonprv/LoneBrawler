// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Code.Common.Extensions.CustomTypes.Types
{
  [Serializable]
  public class DictionaryData<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
  {
    [SerializeField, HideInInspector]
    private List<TKey> keyData = new List<TKey>();

    [SerializeField, HideInInspector]
    private List<TValue> valueData = new List<TValue>();

    public void OnAfterDeserialize()
    {
      Clear();
      for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
      {
        this[keyData[i]] = valueData[i];
      }
    }

    public void OnBeforeSerialize()
    {
      keyData.Clear();
      valueData.Clear();

      foreach (var item in this)
      {
        keyData.Add(item.Key);
        valueData.Add(item.Value);
      }
    }
  }
}
