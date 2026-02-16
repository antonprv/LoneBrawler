// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Manifests
{
  [UnityEngine.CreateAssetMenu(fileName = "LevelsManifest",
    menuName = "StaticData/Manifests/LevelsManifest")]
  public class LevelsManifestStaticData : UnityEngine.ScriptableObject
  {
    public DictionaryData<string, AssetReferenceT<LevelStaticData>> Levels = new();
  }
}
