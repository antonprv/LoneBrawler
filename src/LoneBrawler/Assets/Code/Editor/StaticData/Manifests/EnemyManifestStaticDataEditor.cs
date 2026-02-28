// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using System.Collections.Generic;

using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types.Enemies;
using Code.Editor.Common.Manifests;
using Code.Editor.Common.Manifests.Drawers;
using Code.Editor.Common.Manifests.Interfaces;

using UnityEditor;

using UnityEngine.AddressableAssets;

namespace Code.Editor.StaticData.Manifests
{
  /// <summary>
  /// Custom editor for EnemyManifestStaticData that provides automatic population
  /// of the Enemies dictionary from all available EnemyStaticData assets.
  /// </summary>
  [CustomEditor(typeof(EnemyManifestStaticData))]
  public class EnemyManifestStaticDataEditor : ManifestEditorBase<EnemyManifestStaticData, EnemyStaticData, EnemyTypeId>
  {
    protected override string GetDictionaryPropertyName() =>
      $"{nameof(EnemyManifestStaticData.Enemies)}";

    protected override string GetDictionaryDisplayLabel() =>
      $"{nameof(EnemyManifestStaticData.Enemies)}";

    protected override EnemyTypeId GetKeyFromData(EnemyStaticData data) => data.EnemyTypeId;

    protected override IDictionary<EnemyTypeId, AssetReferenceT<EnemyStaticData>>
        GetDictionary(EnemyManifestStaticData manifest) => manifest.Enemies;

    /// <summary>
    /// Use enum dropdown drawer for EnemyTypeId keys.
    /// </summary>
    protected override ICustomKeyDrawer CreateCustomKeyDrawer()
    {
      return new EnumDropdownKeyDrawer<EnemyTypeId>();
    }
  }
}
#endif
