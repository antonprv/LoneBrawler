// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData.SecondaryData;

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "LevelStaticData",
    menuName = "StaticData/LevelStaticData")]
  public class LevelStaticData : ScriptableObject
  {
    public string LevelKey;

    public List<EnemySpawnerStaticData> EnemySpawners;
  }
}
