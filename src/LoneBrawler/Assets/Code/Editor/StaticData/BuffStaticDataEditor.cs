// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Editor.Common;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(BuffStaticData))]
  public class BuffStaticDataEditor : ManualSaveEditor
  {
    private const int _space = 10;
    private SerializedProperty _buffClassField;
    private SerializedProperty _buffActivationTypeField;
    private SerializedProperty _buffDurationField;
    private SerializedProperty _buffCostField;
    private SerializedProperty _buffFXReference;

    private void OnEnable()
    {
      var buffData = (BuffStaticData)target;

      _buffClassField = serializedObject.FindProperty(nameof(buffData.Class));
      _buffActivationTypeField = serializedObject.FindProperty(nameof(buffData.ActivationType));
      _buffDurationField = serializedObject.FindProperty(nameof(buffData.Duration));
      _buffCostField = serializedObject.FindProperty(nameof(buffData.Cost));
      _buffFXReference = serializedObject.FindProperty(nameof(buffData.BuffEffectPrefab));
    }

    protected override void DrawInspector()
    {
      EditorGUILayout.Space(_space);

      InspectorUtils.DrawFilteredEnumPopup(
        _buffClassField, new GUIContent("Class"), BuffClassName.None, BuffClassName.BuffBase);

      EditorGUILayout.PropertyField(_buffActivationTypeField, new GUIContent("Activation Type"));
      EditorGUILayout.PropertyField(_buffDurationField, new GUIContent("Duration"));
      EditorGUILayout.PropertyField(_buffCostField, new GUIContent("Cost"));

      EditorGUILayout.Space(_space);
      EditorGUILayout.LabelField("FX Prefab Reference", EditorStyles.boldLabel);
      EditorGUILayout.Space(_space);

      EditorGUILayout.PropertyField(_buffFXReference, new GUIContent("Buff Effect Prefab"));
    }
  }
}
