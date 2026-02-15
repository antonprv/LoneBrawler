// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [UnityEngine.CreateAssetMenu(fileName = "WindowStaticData",
  menuName = "StaticData/WindowStaticData")]
  public class WindowStaticData : UnityEngine.ScriptableObject
  {
    public WindowTypeId WindowId;
    public AssetReferenceGameObject WindowReference;
  }
}
