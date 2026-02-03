// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Data.DataExtensions;
using Code.Data.SaveData.Common;

using Unity.VisualScripting;

using UnityEngine;

namespace Code.Data.SaveData
{
  [Serializable]
  public class SoulsCollected : ISerializationCallbackReceiver
  {
    public int Amount;

    [DoNotSerialize]
    public Dictionary<string, Vector3> LeftSpawners;

    [SerializeField]
    private List<string> _leftSpawnersId;
    [SerializeField]
    private List<Vector3Data> _leftSpawnersPositions;

    public SoulsCollected()
    {
      Amount = 0;
      LeftSpawners = new Dictionary<string, Vector3>();

      _leftSpawnersId = new List<string>();
      _leftSpawnersPositions = new List<Vector3Data>();
    }

    public void OnAfterDeserialize()
    {
      LeftSpawners ??= new Dictionary<string, Vector3>();
      LeftSpawners.Clear();

      if (_leftSpawnersId == null || _leftSpawnersPositions == null)
        return;

      int count = Mathf.Min(_leftSpawnersId.Count, _leftSpawnersPositions.Count);

      for (int i = 0; i < count; i++)
      {
        string id = _leftSpawnersId[i];
        Vector3 position = _leftSpawnersPositions[i].AsUnityVector();

        if (string.IsNullOrEmpty(id))
          continue;

        LeftSpawners[id] = position;
      }
    }

    public void OnBeforeSerialize()
    {
      _leftSpawnersId.Clear();
      _leftSpawnersPositions.Clear();
      foreach (var kvp in LeftSpawners)
      {
        _leftSpawnersId.Add(kvp.Key);
        _leftSpawnersPositions.Add(kvp.Value.AsVector3Data());
      }
    }
  }
}
