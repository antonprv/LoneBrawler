// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Types
{
  [System.Serializable]
  public class WindowConfig
  {
    public WindowTypeId windowId;
    public AssetReferenceGameObject windowReference;
  }
}
