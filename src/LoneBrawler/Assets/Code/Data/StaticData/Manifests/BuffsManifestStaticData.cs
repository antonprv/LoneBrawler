// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Data.StaticData.Types.Buff;

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Manifests
{
  [UnityEngine.CreateAssetMenu(fileName = "BuffsManifest",
  menuName = "StaticData/Manifests/BuffsManifest")]
  public class BuffsManifestStaticData : UnityEngine.ScriptableObject
  {
    public DictionaryData<BuffClassName, AssetReferenceT<BuffStaticData>> Buffs = new();
  }
}
