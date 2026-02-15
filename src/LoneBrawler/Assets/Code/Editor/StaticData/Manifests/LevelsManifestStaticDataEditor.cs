// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Editor.Common.Manifests;
using Code.Editor.Common.Manifests.Drawers;
using Code.Editor.Common.Manifests.Interfaces;

using UnityEditor;

using UnityEngine.AddressableAssets;

namespace Code.Editor.StaticData.Manifests
{
  /// <summary>
  /// Custom editor for LevelsManifestStaticData with scene dropdown for keys.
  /// </summary>
  [CustomEditor(typeof(LevelsManifestStaticData))]
  public class LevelsManifestStaticDataEditor :
    ManifestEditorBase<LevelsManifestStaticData, LevelStaticData, string>
  {
    protected override string GetDictionaryPropertyName() =>
      $"{nameof(LevelsManifestStaticData.Levels)}";

    protected override string GetDictionaryDisplayLabel() =>
      $"{nameof(LevelsManifestStaticData.Levels)}";

    protected override string GetKeyFromData(LevelStaticData data) => data.LevelKey;

    protected override System.Collections.Generic.IDictionary<string, AssetReferenceT<LevelStaticData>>
        GetDictionary(LevelsManifestStaticData manifest) => manifest.Levels;

    /// <summary>
    /// Use scene dropdown drawer for string keys.
    /// </summary>
    protected override ICustomKeyDrawer CreateCustomKeyDrawer() => new SceneDropdownKeyDrawer();
  }
}
#endif
