// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes.Types;
using Code.Data.StaticData.Types;

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [UnityEngine.CreateAssetMenu(fileName = "EnemyManifest",
    menuName = "StaticData/EnemyManifest")]
  public class EnemyManifestStaticData : UnityEngine.ScriptableObject
  {
    public DictionaryData<EnemyTypeId, AssetReferenceT<EnemyStaticData>> Enemies;
  }
}
