// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Data.StaticData.Types.Buff;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData.Types
{
  [CustomPropertyDrawer(typeof(BuffParameterValue))]
  public class BuffParameterValueDrawer : PropertyDrawer
  {
    private const float TYPE_WIDTH = 80f;
    private const float SPACING = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
      EditorGUIUtility.singleLineHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);

      SerializedProperty typeProp = property.FindPropertyRelative("Type");

      // Type dropdown — fixed width on the left
      Rect typeRect = new Rect(
        position.x,
        position.y,
        TYPE_WIDTH,
        position.height);

      EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);

      // Active value field — fills the rest
      Rect valueRect = new Rect(
        position.x + TYPE_WIDTH + SPACING,
        position.y,
        position.width - TYPE_WIDTH - SPACING,
        position.height);

      SerializedProperty valueProp = GetActiveValueProperty(
        property,
        (BuffParameterType)typeProp.enumValueIndex);

      if (valueProp != null)
        EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

      EditorGUI.EndProperty();
    }

    private static SerializedProperty GetActiveValueProperty(
      SerializedProperty parent,
      BuffParameterType type) => type switch
      {
        BuffParameterType.Int => parent.FindPropertyRelative("_intValue"),
        BuffParameterType.Float => parent.FindPropertyRelative("_floatValue"),
        BuffParameterType.Bool => parent.FindPropertyRelative("_boolValue"),
        BuffParameterType.String => parent.FindPropertyRelative("_stringValue"),
        BuffParameterType.AssetReference => parent.FindPropertyRelative("_assetReferenceValue"),
        _ => null
      };
  }
}
#endif
