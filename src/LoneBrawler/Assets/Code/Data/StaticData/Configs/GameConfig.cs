// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData.Configs
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "StaticData/Config/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    // Gameplay Tag Settings
    public string PlayerStartTag;
    public string EnemySpawnerTag;

    // Physics Tags Settings
    public int PlayerLayer;
    public int EnemyHitableLayer;
    public int LootLayer;
  }
}
