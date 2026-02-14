// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.
using Code.Common.Extensions.CustomTypes.Types;

using UnityEngine;

namespace Code.Data.SaveData
{
  [System.Serializable]
  public class SoulsCollected
  {
    public int Amount;

    public DictionaryData<string, Vector3> LeftSpawners;

    public SoulsCollected()
    {
      Amount = 0;
      LeftSpawners = new DictionaryData<string, Vector3>();
    }
  }
}
