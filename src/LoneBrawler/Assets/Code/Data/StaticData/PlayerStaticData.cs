// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Configs;

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "PlayerData", menuName = "StaticData/PlayerData")]
  public class PlayerStaticData : ScriptableObject
  {
    // Player Settings
    [Range(1f, 699)]
    public float PlayerMaxHealth;
    [Range(1f, 699)]
    public float PlayerAttackDamage;
    [Range(0.1f, 1)]
    public float PlayerAttackRange;
    [Range(0.1f, 1)]
    public float PlayerAttackRadius;

    [Range(1, 50)]
    public int PlayerMaxEnemiesHit;
  }

  public static class PlayerDataAcessor
  {
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
    private static PlayerStaticData _playerData;

    private static PlayerStaticData GetConfiguration()
    {
      if (!_playerData)
      {
        _playerData = Resources.Load<PlayerStaticData>("StaticData/PlayerData");

        if (!_playerData)
        {
          Debug.LogError("PlayerData not found! Make sure it's in a Resources folder with correct path");
          _playerData = ScriptableObject.CreateInstance<PlayerStaticData>();
        }
      }
      return _playerData;
    }
  }
}

