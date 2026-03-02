// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(BuffStaticData))]
  public class BuffStaticDataEditor : ManualSaveEditor
  {
    private bool _configData = true;
    private bool _uiData = true;
    private bool _dynamicData = true;

    private const int FoldoutSpaces = 10;

    protected override void DrawInspector()
    {
      InspectorUtils.DrawFoldout(
        serializedObject,
        "Buff Configuration",
        ref _configData,
        ConfigFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "UI / Inventory Display",
        ref _uiData,
        UIFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Dynamic Parameters",
        ref _dynamicData,
        DynamicFields);
    }

    private static readonly string[] ConfigFields =
    {
      nameof(BuffStaticData.Class),
      nameof(BuffStaticData.ActivationType),
      nameof(BuffStaticData.Duration),
      nameof(BuffStaticData.Cost),
      nameof(BuffStaticData.BuffEffectPrefab)
    };

    private static readonly string[] UIFields =
    {
      nameof(BuffStaticData.DisplayName),
      nameof(BuffStaticData.Description),
      nameof(BuffStaticData.Icon),
      nameof(BuffStaticData.ShopItemPrefabReference),
      nameof(BuffStaticData.AmountInShop),
      nameof(BuffStaticData.MaxStack)
    };

    private static readonly string[] DynamicFields =
    {
      nameof(BuffStaticData.DynamicParameters)
    };
  }
}
