// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;

using Code.Common.CustomTypes.Domain.Collections;

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Manifests
{
  [UnityEngine.CreateAssetMenu(fileName = "WindowsManifest",
    menuName = "StaticData/Manifests/WindowsManifest")]
  public class WindowsManifestStaticData : UnityEngine.ScriptableObject
  {
    public DictionaryData<WindowTypeId, AssetReferenceT<WindowStaticData>> Windows = new();
  }
}
