// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Configs;

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "PlayerStaticData", menuName = "StaticData/PlayerStaticData")]
  public class PlayerStaticData : ScriptableObject
  {
    [Range(1f, 699)] public float PlayerMaxHealth = 100f;

    [Range(1f, 699)] public float PlayerAttackDamage = 1f;

    [Range(0.1f, 1)] public float PlayerAttackRange = 0.8f;

    [Range(0.1f, 1)] public float PlayerAttackRadius = 0.7f;


    [Range(1, 50)] public int PlayerMaxEnemiesHit = 3;

  }
}

