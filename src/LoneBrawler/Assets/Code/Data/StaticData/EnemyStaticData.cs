// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "EnemyStaticData", menuName = "StaticData/EnemyStaticData")]
  public class EnemyStaticData : ScriptableObject
  {
    public EnemyTypeId EnemyTypeId;

    public int hp;
  }
}
