// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData.Configs
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "StaticData/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    // Global Settings
    public string PlayerTag;
    public string PlayerStartTag;
    public string EnemySpawnerTag;

    public int PlayerLayer;
    public int EnemyHitableLayer;
  }
}
