// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Attributes;
using Code.Data.StaticData.Configs.Types;
using Code.Data.StaticData.Types;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "EnemyStaticData",
    menuName = "StaticData/EnemyStaticData")]
  public class EnemyStaticData : ScriptableObject
  {
    public EnemyTypeId EnemyTypeId;

    // ──────────────────────────────────────────────
    //  Attack
    // ──────────────────────────────────────────────
    [Header("Attack")]
    [NoNone] public EnemyAttackType EnemyAttackType;

    /// <summary>
    /// Addressables reference to the attack preset.
    /// Loaded via IEnemyDataSubservice.ForAttackPresetAsync() so that
    /// AssetLoader can cache it by GUID — the same preset (e.g. "Fireball")
    /// lives in memory as a single instance regardless of how many enemies use it.
    /// </summary>
    public AssetReferenceT<AttackPresetStaticData> AttackPresetReference;

    [Header("Attack Behavior")]
    [Range(0f, 5f)] public float AttackCooldown = 0.3f;
    [Range(0.5f, 5f)] public float HitRecoverCooldown = 1.5f;
    [Range(1f, 5f)] public float AttackTurnSpeed = 5f;

    // ──────────────────────────────────────────────
    //  Health / Death / Movement / Loot
    // ──────────────────────────────────────────────
    [Header("Health")]
    [Range(1f, 699f)] public float MaxHealth = 50f;

    [Range(0.1f, 699)] public float DisappearDelay;

    [Header("Movement")]
    [Range(0f, 20f)] public float ReachDistance = 1f;
    [Range(0f, 1600f)] public float AngularSpeed = 1600f;
    [Range(0f, 5f)] public float Speed = 2f;

    [Header("Loot")]
    [Range(0, 699)] public int SoulsMin;
    [Range(0, 699)] public int SoulsMax;

    public AssetReferenceGameObject PrefabReference;
    public AssetReferenceGameObject LootPrefabReference;
  }
}
