// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace Code.Common.Extensions.CustomTypes.Types.Editor
{
  [CustomPropertyDrawer(typeof(DictionaryData<,>))]
  public class DictionaryDataDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);

      property.isExpanded = EditorGUI.Foldout(
        new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
        property.isExpanded,
        label,
        true
      );

      if (property.isExpanded)
      {
        EditorGUI.indentLevel++;

        var keyDataProperty = property.FindPropertyRelative("keyData");
        var valueDataProperty = property.FindPropertyRelative("valueData");

        if (keyDataProperty != null && valueDataProperty != null)
        {
          int count = Mathf.Max(keyDataProperty.arraySize, valueDataProperty.arraySize);
          float yOffset = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

          // Size field
          int newSize = EditorGUI.IntField(
            new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight),
            "Size",
            count
          );

          if (newSize != count)
          {
            keyDataProperty.arraySize = newSize;
            valueDataProperty.arraySize = newSize;
          }

          yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

          // Draw key-value pairs
          for (int i = 0; i < count; i++)
          {
            var keyElement = keyDataProperty.GetArrayElementAtIndex(i);
            var valueElement = valueDataProperty.GetArrayElementAtIndex(i);

            float keyHeight = EditorGUI.GetPropertyHeight(keyElement);
            float valueHeight = EditorGUI.GetPropertyHeight(valueElement);

            Rect elementRect = new Rect(position.x, yOffset, position.width, Mathf.Max(keyHeight, valueHeight));

            // Draw element label
            EditorGUI.LabelField(
              new Rect(elementRect.x, elementRect.y, position.width, EditorGUIUtility.singleLineHeight),
              $"Element {i}"
            );

            float elementYOffset = elementRect.y + EditorGUIUtility.singleLineHeight + 2;

            // Draw key
            EditorGUI.PropertyField(
              new Rect(elementRect.x, elementYOffset, position.width, keyHeight),
              keyElement,
              new GUIContent("Key"),
              true
            );

            elementYOffset += keyHeight + 2;

            // Draw value
            EditorGUI.PropertyField(
              new Rect(elementRect.x, elementYOffset, position.width, valueHeight),
              valueElement,
              new GUIContent("Value"),
              true
            );

            yOffset += EditorGUIUtility.singleLineHeight + keyHeight + valueHeight + 6;
          }
        }

        EditorGUI.indentLevel--;
      }

      EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      if (!property.isExpanded)
      {
        return EditorGUIUtility.singleLineHeight;
      }

      var keyDataProperty = property.FindPropertyRelative("keyData");
      var valueDataProperty = property.FindPropertyRelative("valueData");

      if (keyDataProperty == null || valueDataProperty == null)
      {
        return EditorGUIUtility.singleLineHeight;
      }

      float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Foldout
      height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Size field

      int count = Mathf.Max(keyDataProperty.arraySize, valueDataProperty.arraySize);

      for (int i = 0; i < count; i++)
      {
        var keyElement = keyDataProperty.GetArrayElementAtIndex(i);
        var valueElement = valueDataProperty.GetArrayElementAtIndex(i);

        height += EditorGUIUtility.singleLineHeight + 2; // Element label
        height += EditorGUI.GetPropertyHeight(keyElement, true) + 2; // Key
        height += EditorGUI.GetPropertyHeight(valueElement, true) + 2; // Value
        height += 2; // Spacing
      }

      return height;
    }
  }
}
#endif
