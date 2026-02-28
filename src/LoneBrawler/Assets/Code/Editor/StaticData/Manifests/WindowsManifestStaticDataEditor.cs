// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types.UI;
using Code.Editor.Common.Manifests;
using Code.Editor.Common.Manifests.Drawers;
using Code.Editor.Common.Manifests.Interfaces;

using UnityEditor;

using UnityEngine.AddressableAssets;

namespace Code.Editor.StaticData.Manifests
{
  [CustomEditor(typeof(WindowsManifestStaticData))]
  public class WindowsManifestStaticDataEditor : ManifestEditorBase<WindowsManifestStaticData, WindowStaticData, WindowTypeId>
  {
    protected override string GetDictionaryPropertyName() =>
      $"{nameof(WindowsManifestStaticData.Windows)}";

    protected override string GetDictionaryDisplayLabel() =>
      $"{nameof(WindowsManifestStaticData.Windows)}";

    protected override WindowTypeId GetKeyFromData(WindowStaticData data) => data.WindowId;

    protected override IDictionary<WindowTypeId, AssetReferenceT<WindowStaticData>> GetDictionary(WindowsManifestStaticData manifest) => manifest.Windows;

    protected override ICustomKeyDrawer CreateCustomKeyDrawer() =>
      new EnumDropdownKeyDrawer<WindowTypeId>();
  }
}
