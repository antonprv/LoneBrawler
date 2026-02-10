// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.
using System;
using System.Collections.Generic;

using Code.Common.Extensions.CustomTypes;
using Code.Common.Extensions.CustomTypes.Types;

using UnityEngine;

namespace Code.Data.SaveData
{
  [Serializable]
  public class SoulsCollected : ISerializationCallbackReceiver
  {
    public int Amount;

    [NonSerialized]
    public Dictionary<string, Vector3> LeftSpawners;

    [SerializeField]
    private List<PairData<string, Vector3Data>> _leftSpawners;

    public SoulsCollected()
    {
      Amount = 0;
      LeftSpawners = new Dictionary<string, Vector3>();
      _leftSpawners = new List<PairData<string, Vector3Data>>();
    }
    public void OnAfterDeserialize()
    {
      InitializeLeftSpawners();
      DeserializeLeftSpawners();
    }

    public void OnBeforeSerialize() =>
      SerializeLeftSpawners();

    private void InitializeLeftSpawners()
    {
      LeftSpawners ??= new Dictionary<string, Vector3>();
      LeftSpawners.Clear();
    }

    private void DeserializeLeftSpawners()
    {
      if (_leftSpawners == null)
        return;

      foreach (var pair in _leftSpawners)
        if (IsValidSpawnerKey(pair.Key))
          AddSpawnerToDictionary(pair);
    }

    private void SerializeLeftSpawners()
    {
      _leftSpawners ??= new List<PairData<string, Vector3Data>>();
      _leftSpawners.Clear();

      foreach (var spawner in LeftSpawners)
        AddSpawnerToList(spawner);
    }

    private static bool IsValidSpawnerKey(string key) =>
      !string.IsNullOrEmpty(key);

    private void AddSpawnerToDictionary(PairData<string, Vector3Data> pair)
    {
      Vector3 position = pair.Value.AsUnityVector();
      LeftSpawners[pair.Key] = position;
    }

    private void AddSpawnerToList(KeyValuePair<string, Vector3> spawner)
    {
      var pair = new PairData<string, Vector3Data>(
          spawner.Key,
          spawner.Value.AsVector3Data()
      );
      _leftSpawners.Add(pair);
    }
  }
}
