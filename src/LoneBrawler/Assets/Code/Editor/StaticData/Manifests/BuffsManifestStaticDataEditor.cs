// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

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
  [CustomEditor(typeof(BuffsManifestStaticData))]
  public class BuffsManifestStaticDataEditor : ManifestEditorBase<BuffsManifestStaticData, BuffStaticData, BuffClassName>
  {
    protected override IDictionary<BuffClassName, AssetReferenceT<BuffStaticData>>
      GetDictionary(BuffsManifestStaticData manifest) => manifest.Buffs;

    protected override string GetDictionaryDisplayLabel() => $"{nameof(BuffsManifestStaticData.Buffs)}";

    protected override string GetDictionaryPropertyName() => $"{nameof(BuffsManifestStaticData.Buffs)}";

    protected override BuffClassName GetKeyFromData(BuffStaticData data) => data.Class;

    protected override ICustomKeyDrawer CreateCustomKeyDrawer()
    {
      return new EnumDropdownKeyDrawer<BuffClassName>();
    }
  }
}
