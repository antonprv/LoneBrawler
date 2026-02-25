// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Attributes;
using Code.Data.StaticData.Types.Buff;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "BuffStaticData",
  menuName = "StaticData/Buffs")]
  public class BuffStaticData : ScriptableObject
  {
    [NoNone] public BuffClassName Class = BuffClassName.BuffBase;
    [NoNone] public BuffActivationType ActivationType = BuffActivationType.Burst;

    [Range(1f, 699f)] public float Duration = 1f;
    [Range(1, 699)] public int Cost = 1;

    public AssetReferenceGameObject BuffEffectPrefab;
  }
}
