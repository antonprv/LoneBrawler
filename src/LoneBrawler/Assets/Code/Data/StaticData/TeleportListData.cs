// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes.Types;

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "EnemyStaticData",
  menuName = "StaticData/EnemyStaticData")]
  public class TeleportListData : ScriptableObject
  {
    public DictionaryData<string, PairData<string, string>> TeleportList =
      new DictionaryData<string, PairData<string, string>>();
  }
}
