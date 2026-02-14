// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEngine;

namespace Code.Common.Extensions.CustomTypes.Types
{
  /// <summary>
  /// Serializable dictionary for Unity that maintains synchronization between 
  /// dictionary data and serialized lists for Inspector editing.
  /// </summary>
  [System.Serializable]
  public class DictionaryData<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
  {
    [SerializeField]
    private List<TKey> keyData = new List<TKey>();

    [SerializeField]
    private List<TValue> valueData = new List<TValue>();

    [System.NonSerialized]
    private bool _needsSerialization = false;

    #region ISerializationCallbackReceiver Implementation

    /// <summary>
    /// Deserializes data from Unity serialized lists into the dictionary.
    /// Called by Unity after deserialization.
    /// </summary>
    public void OnAfterDeserialize()
    {
      RebuildDictionaryFromSerializedData();
      _needsSerialization = false;
    }

    /// <summary>
    /// Serializes dictionary data into Unity-compatible lists.
    /// Called by Unity before serialization.
    /// </summary>
    public void OnBeforeSerialize()
    {
      if (ShouldSkipSerialization())
        return;

      SynchronizeListsWithDictionary();
    }

    #endregion

    #region Dictionary Method Overrides

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// Marks the dictionary as modified for serialization.
    /// </summary>
    public new TValue this[TKey key]
    {
      get => base[key];
      set
      {
        base[key] = value;
        MarkAsModified();
      }
    }

    /// <summary>
    /// Adds a key-value pair to the dictionary.
    /// </summary>
    public new void Add(TKey key, TValue value)
    {
      base.Add(key, value);
      MarkAsModified();
    }

    /// <summary>
    /// Removes the value with the specified key from the dictionary.
    /// </summary>
    public new bool Remove(TKey key)
    {
      bool wasRemoved = base.Remove(key);
      if (wasRemoved)
        MarkAsModified();

      return wasRemoved;
    }

    /// <summary>
    /// Removes all keys and values from the dictionary.
    /// </summary>
    public new void Clear()
    {
      base.Clear();
      MarkAsModified();
    }

    /// <summary>
    /// Attempts to add the specified key and value to the dictionary.
    /// </summary>
    public new bool TryAdd(TKey key, TValue value)
    {
      bool wasAdded = base.TryAdd(key, value);
      if (wasAdded)
        MarkAsModified();

      return wasAdded;
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Rebuilds the dictionary from serialized key-value lists.
    /// Skips null keys and duplicate entries.
    /// </summary>
    private void RebuildDictionaryFromSerializedData()
    {
      Clear();

      int pairCount = Mathf.Min(keyData.Count, valueData.Count);

      for (int i = 0; i < pairCount; i++)
      {
        TKey key = keyData[i];

        if (IsValidKey(key) && !ContainsKey(key))
        {
          base[key] = valueData[i];
        }
      }
    }

    /// <summary>
    /// Synchronizes serialized lists with current dictionary state.
    /// </summary>
    private void SynchronizeListsWithDictionary()
    {
      keyData.Clear();
      valueData.Clear();

      foreach (var pair in this)
      {
        keyData.Add(pair.Key);
        valueData.Add(pair.Value);
      }
    }

    /// <summary>
    /// Determines if serialization should be skipped in the current context.
    /// In editor mode (not playing), PropertyDrawer handles serialization directly.
    /// </summary>
    private bool ShouldSkipSerialization()
    {
#if UNITY_EDITOR
      if (!Application.isPlaying && !_needsSerialization)
        return true;
#endif
      return false;
    }

    /// <summary>
    /// Validates that a key is not null and can be used in the dictionary.
    /// </summary>
    private bool IsValidKey(TKey key)
    {
      // For reference types, check for null
      if (typeof(TKey).IsClass || typeof(TKey).IsInterface)
        return key != null;

      return true;
    }

    /// <summary>
    /// Marks the dictionary as modified, requiring serialization.
    /// </summary>
    private void MarkAsModified()
    {
      _needsSerialization = true;
    }

    #endregion
  }
}
