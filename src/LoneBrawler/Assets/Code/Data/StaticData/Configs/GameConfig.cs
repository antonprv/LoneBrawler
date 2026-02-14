// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Data.StaticData.Configs
{
  [UnityEngine.CreateAssetMenu(fileName = "GameConfig", menuName = "StaticData/Config/GameConfig")]
  public class GameConfig : UnityEngine.ScriptableObject
  {
    // Gameplay Tags
    public string PlayerTag;
    public string PlayerStartTag;
    public string EnemyTag;
    public string EnemySpawnerTag;

    // Metadata Layers
    public int PlayerLayer;
    public int EnemyHitableLayer;
    public int LootLayer;
    public int AggroLayer;
    public int AttackZoneLayer;
    public int SaveTriggerLayer;
  }
}
