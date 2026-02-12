// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Types
{
  [Serializable]
  public class WindowConfig
  {
    public WindowTypeId windowId;
    public AssetReferenceGameObject windowReference;
  }
}
