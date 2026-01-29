// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "EnemyStaticData", menuName = "StaticData/EnemyStaticData")]
  public class EnemyStaticData : ScriptableObject
  {
    public EnemyTypeId EnemyTypeId;

    [Range(0f, 0.5f)] public float AttackCooldown = 0.3f;

    [Range(0.1f, 1f)] public float HitRadius = 0.7f;

    [Range(0.1f, 1f)] public float HitRange = 0.8f;

    [Range(1f, 5f)] public float AttackTurnSpeed = 5f;

    [Range(1f, 699f)] public float attackDamage = 10f;

    [Range(1f, 699f)] public float MaxHealth = 50f;
  }
}
