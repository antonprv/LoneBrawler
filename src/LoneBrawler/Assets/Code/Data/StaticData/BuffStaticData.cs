// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Data.StaticData.Types.Buff;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "BuffStaticData", menuName = "StaticData/Buff")]
  public class BuffStaticData : ScriptableObject
  {
    [Header("Buff Configuration")]
    [FilteredEnum(BuffClassName.None, BuffClassName.BuffBase)]
    public BuffClassName Class = BuffClassName.BuffBase;

    [FilteredEnum(BuffActivationType.None)]
    public BuffActivationType ActivationType = BuffActivationType.Burst;

    [Range(1f, 699f)] public float Duration = 1f;
    [Range(1, 699)] public int Cost = 1;

    public AssetReferenceGameObject BuffEffectPrefab;

    [Header("UI / Inventory Display")]
    [Tooltip("Display name in inventory and UI")]
    public string DisplayName;

    [Tooltip("Description shown in tooltips")]
    [TextArea(2, 4)]
    public string Description;

    [Tooltip("Icon for inventory slots and UI")]
    public AssetReferenceSprite Icon;

    [Tooltip("UI element prefab for instantiation in shop")]
    public AssetReferenceGameObject ShopItemPrefabReference;

    [Tooltip("Amount sold in shop bundle")]
    [Range(1, 699)] public int AmountInShop = 8;

    [Tooltip("Maximum stack size in inventory")]
    [Range(1, 699)]
    public int MaxStack = 64;

    [Tooltip("Arbitrary typed key-value pairs. Add new entries in the inspector.")]
    public DictionaryData<string, BuffParameterValue> DynamicParameters = new();
  }
}
