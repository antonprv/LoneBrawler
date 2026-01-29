// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Configs
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    // Global Settings
    public string PlayerTag;
    public string PlayerStartTag;

    public float EnemyDisappearDelay;

    public int PlayerLayer;
    public int EnemyHitableLayer;

    // Player Settings
    public float PlayerMaxHealth;
    public float PlayerAttackDamage;
    public float PlayerAttackRange;
    public float PlayerAttackRadius;

    public int PlayerMaxEnemiesHit;
  }

  public static class GameConfiguration
  {
    //=========================================================================
    // Global Settings
    //=========================================================================
    // Tags
    public static string PlayerTag => GetConfiguration().PlayerTag;
    public static string PlayerStartTag => GetConfiguration().PlayerStartTag;

    // Delays
    public static float EnemyDisappearDelay => GetConfiguration().EnemyDisappearDelay;

    // Physics Layers
    public static int PlayerCollision => 1 << GetConfiguration().PlayerLayer;
    public static int EnemyHitableLayer => 1 << GetConfiguration().EnemyHitableLayer;

    //=========================================================================
    // Player Settings
    //=========================================================================
    public static float PlayerMaxHealth => GetConfiguration().PlayerMaxHealth;
    public static float PlayerAttackDamage => GetConfiguration().PlayerAttackDamage;
    public static float PlayerAttackRange => GetConfiguration().PlayerAttackRange;
    public static float PlayerAttackRadius => GetConfiguration().PlayerAttackRadius;
    public static int PlayerMaxEnemiesHit => GetConfiguration().PlayerMaxEnemiesHit;

    //=========================================================================
    // Constructor
    //=========================================================================
    private static GameConfig _gameconfig;

    private static GameConfig GetConfiguration()
    {
      if (!_gameconfig)
      {
        _gameconfig = Resources.Load<GameConfig>("Config/GameConfig");

        if (!_gameconfig)
        {
          Debug.LogError("GameConfig not found! Make sure it's in a Resources folder with correct path");
          _gameconfig = ScriptableObject.CreateInstance<GameConfig>();
        }
      }

      return _gameconfig;
    }
  }
}
