// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using System.Collections.Generic;

using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types.Buff;
using Code.Editor.Common.Manifests;
using Code.Editor.Common.Manifests.Drawers;
using Code.Editor.Common.Manifests.Interfaces;

using UnityEditor;

using UnityEngine.AddressableAssets;

namespace Code.Editor.StaticData.Manifests
{
  [CustomEditor(typeof(ShopItemsManifestStaticData))]
  public class ShopItemsManifestStaticDataEditor : ManifestEditorBase<ShopItemsManifestStaticData, ShopItemStaticData, BuffClassName>
  {
    protected override IDictionary<BuffClassName, AssetReferenceT<ShopItemStaticData>>
      GetDictionary(ShopItemsManifestStaticData manifest) => manifest.ShopItems;

    protected override string GetDictionaryDisplayLabel() => $"{nameof(ShopItemsManifestStaticData.ShopItems)}";

    protected override string GetDictionaryPropertyName() => $"{nameof(ShopItemsManifestStaticData.ShopItems)}";

    protected override BuffClassName GetKeyFromData(ShopItemStaticData data) => data.BuffClass;

    protected override ICustomKeyDrawer CreateCustomKeyDrawer()
    {
      return new EnumDropdownKeyDrawer<BuffClassName>();
    }
  }
}
#endif
