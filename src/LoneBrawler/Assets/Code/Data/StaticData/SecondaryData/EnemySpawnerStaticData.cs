// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;

using UnityEngine;

namespace Code.Data.StaticData.SecondaryData
{
  [CreateAssetMenu(fileName = "EnemySpawnerStaticData",
    menuName = "StaticData/SecondaryData/EnemySpawnerStaticData")]
  public class EnemySpawnerStaticData : ScriptableObject
  {
    public string SpawnerId;
    public EnemyTypeId EnemyTypeId;
    public Vector3 Position;
  }
}
