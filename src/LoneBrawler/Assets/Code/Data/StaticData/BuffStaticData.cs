// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

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
    public Sprite Icon;

    [Tooltip("Maximum stack size in inventory")]
    [Range(1, 999)]
    public int MaxStack = 1;
  }
}
