// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData.Configs
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "StaticData/Config/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    // Gameplay Tags
    public string PlayerTag;
    public string PlayerStartTag;

    // Metadata Layers
    public int PlayerLayer;
    public int EnemyHitableLayer;
    public int LootLayer;
    public int AggroLayer;
    public int AttackZoneLayer;
  }
}
