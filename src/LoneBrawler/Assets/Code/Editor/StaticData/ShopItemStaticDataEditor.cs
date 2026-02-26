// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Editor.Common;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(ShopItemStaticData))]
  public class ShopItemStaticDataEditor : ManualSaveEditor
  {
    private const int _space = 10;
    private SerializedProperty _shopIconReference;
    private SerializedProperty _shopItemPrefabReference;
    private SerializedProperty _buffClass;

    private void OnEnable()
    {
      var shopItem = (ShopItemStaticData)target;

      _shopIconReference = serializedObject.FindProperty(nameof(shopItem.ShopIconReference));
      _shopItemPrefabReference = serializedObject.FindProperty(nameof(shopItem.ShopItemPrefabReference));
      _buffClass = serializedObject.FindProperty(nameof(shopItem.BuffClass));
    }

    protected override void DrawInspector()
    {
      // References Header
      EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

      EditorGUILayout.PropertyField(
        _shopIconReference,
        new GUIContent("Shop Icon Reference", "Buff icon for display in shop"));

      EditorGUILayout.PropertyField(
        _shopItemPrefabReference,
        new GUIContent("Shop Item Prefab Reference", "UI element prefab for instantiation in shop"));

      EditorGUILayout.Space(_space);

      // Buff Data Header
      EditorGUILayout.LabelField("Buff Data", EditorStyles.boldLabel);

      // Draw enum popup excluding BuffBase
      InspectorUtils.DrawFilteredEnumPopup(
        _buffClass,
        new GUIContent("Buff Class", "Buff class corresponding to this shop item"),
        BuffClassName.None, BuffClassName.BuffBase);
    }
  }
}
