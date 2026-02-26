// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Attributes;
using Code.Data.StaticData.Types.Buff;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "ShopItemStaticData",
  menuName = "StaticData/Shop/ShopItem")]
  public class ShopItemStaticData : ScriptableObject
  {
    [Header("References")]
    [Tooltip("Buff icon for display in shop")]
    public AssetReferenceSprite ShopIconReference;

    [Tooltip("UI element prefab for instantiation in shop")]
    public AssetReferenceGameObject ShopItemPrefabReference;

    [Header("Buff Data")]
    [Tooltip("Buff class corresponding to this shop item")]
    [NoNone] public BuffClassName BuffClass = BuffClassName.BuffBase;
  }
}
