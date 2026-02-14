// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [UnityEngine.CreateAssetMenu(fileName = "EnemyStaticData",
    menuName = "StaticData/EnemyStaticData")]
  public class EnemyStaticData : UnityEngine.ScriptableObject
  {
    public EnemyTypeId EnemyTypeId;

    // IAttacker
    [Range(0.1f, 1f)] public float AttackRadius = 0.7f;
    [Range(0.1f, 1f)] public float AttackRange = 0.8f;
    [Range(1f, 699f)] public float AttackDamage = 10f;
    [Range(1, 10)] public int AttackMaxHit = 1;

    // IEnemyAttacker
    [Range(0f, 5f)] public float AttackCooldown = 0.3f;
    [Range(0.5f, 5f)] public float HitRecoverCooldown = 1.5f;
    [Range(1f, 5f)] public float AttackTurnSpeed = 5f;

    // IEnemyHealth
    [Range(1f, 699f)] public float MaxHealth = 50f;

    // IEnemyDeath
    [Range(0.1f, 699)] public float DisappearDelay;

    // IMovableAgent
    [Range(0f, 20f)] public float ReachDistance = 1f;
    [Range(0f, 1600f)] public float AngularSpeed = 1600f;
    [Range(0f, 5f)] public float Speed = 2f;

    // Souls
    [Range(0, 699)] public int SoulsMin;
    [Range(0, 699)] public int SoulsMax;

    public AssetReferenceGameObject PrefabReference;
    public AssetReferenceGameObject LootPrefabReference;
  }
}
