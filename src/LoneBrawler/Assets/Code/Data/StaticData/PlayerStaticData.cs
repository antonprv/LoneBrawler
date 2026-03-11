// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "PlayerStaticData", menuName = "StaticData/PlayerStaticData")]
  public class PlayerStaticData : ScriptableObject
  {
    // Health
    [Range(1f, 699f)] public float PlayerMaxHealth = 100f;

    // Attack
    [Range(1f, 699f)] public float PlayerAttackDamage = 1f;
    [Range(0.1f, 1f)] public float PlayerAttackRange = 0.8f;
    [Range(0.1f, 1f)] public float PlayerAttackRadius = 0.7f;
    [Range(1, 50)] public int PlayerMaxEnemiesHit = 3;

    // Movement
    [Range(1, 699)] public float MovementSpeed = 4.0f;
    [Range(1, 699)] public float RotationSpeed = 12.0f;

    // Death
    [Range(0.1f, 699f)] public float DeathDelay = 0.1f;
  }
}

