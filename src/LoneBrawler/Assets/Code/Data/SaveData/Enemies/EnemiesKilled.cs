// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

namespace Code.Data.SaveData.Enemies
{
  [Serializable]
  public class EnemiesKilled : ISerializationCallbackReceiver
  {
    [DoNotSerialize]
    public HashSet<string> ClearedSpawners;

    [SerializeField]
    private List<string> _clearedSpawnersSerializable;

    public EnemiesKilled()
    {
      ClearedSpawners = new HashSet<string>();

      _clearedSpawnersSerializable = new List<string>();
    }

    public void OnAfterDeserialize()
    {
      if (_clearedSpawnersSerializable == null) return;

      ClearedSpawners.Clear();
      foreach (var id in _clearedSpawnersSerializable)
        ClearedSpawners.Add(id);
    }

    public void OnBeforeSerialize()
    {
      if (_clearedSpawnersSerializable == null
        || _clearedSpawnersSerializable.Count == 0)
        return;

      _clearedSpawnersSerializable.Clear();
      _clearedSpawnersSerializable.AddRange(ClearedSpawners);
    }
  }
}
