// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace Code.Common.Extensions.CustomTypes.Types.Editor
{
  [CustomPropertyDrawer(typeof(Code.Common.CustomTypes.Domain.Collections.DictionaryData<,>))]
  public class DictionaryDataDrawer : PropertyDrawer
  {
    private const float COLUMN_SPACING = 10f;
    private const float VERTICAL_PADDING = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      DrawDictionary(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var list = CreateReorderableList(property, label);
      return list.GetHeight();
    }

    private void DrawDictionary(Rect position, SerializedProperty property, GUIContent label)
    {
      EnsureArraySynchronization(property);

      var list = CreateReorderableList(property, label);
      list.DoList(position);
    }

    private ReorderableList CreateReorderableList(SerializedProperty property, GUIContent label)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");

      var list = new ReorderableList(
          property.serializedObject,
          keyArray,
          draggable: true,
          displayHeader: true,
          displayAddButton: true,
          displayRemoveButton: true
      );

      list.drawHeaderCallback = rect => DrawHeader(rect, label.text);
      list.elementHeightCallback = index => CalculateElementHeight(property, index);
      list.drawElementCallback = (rect, index, isActive, isFocused) =>
          DrawElement(rect, property, index);

      list.onAddCallback = _ => AddElement(property);
      list.onRemoveCallback = l => RemoveElement(property, l.index);

      list.headerHeight = EditorGUIUtility.singleLineHeight * 2 + VERTICAL_PADDING * 2;

      return list;
    }

    private void DrawHeader(Rect rect, string title)
    {
      Rect titleRect = new Rect(
          rect.x,
          rect.y,
          rect.width,
          EditorGUIUtility.singleLineHeight
      );

      EditorGUI.LabelField(titleRect, title, EditorStyles.boldLabel);

      float columnWidth = (rect.width - COLUMN_SPACING) / 2f;
      float labelY = rect.y + EditorGUIUtility.singleLineHeight + VERTICAL_PADDING;

      EditorGUI.LabelField(
          new Rect(rect.x, labelY, columnWidth, EditorGUIUtility.singleLineHeight),
          "Key",
          EditorStyles.miniLabel
      );

      EditorGUI.LabelField(
          new Rect(rect.x + columnWidth + COLUMN_SPACING, labelY, columnWidth, EditorGUIUtility.singleLineHeight),
          "Value",
          EditorStyles.miniLabel
      );
    }

    private void DrawElement(Rect rect, SerializedProperty property, int index)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (!IsValidIndex(index, keyArray, valueArray))
        return;

      SerializedProperty keyElement = keyArray.GetArrayElementAtIndex(index);
      SerializedProperty valueElement = valueArray.GetArrayElementAtIndex(index);

      float columnWidth = (rect.width - COLUMN_SPACING) / 2f;
      rect.y += EditorGUIUtility.standardVerticalSpacing;

      float keyHeight = EditorGUI.GetPropertyHeight(keyElement, true);
      float valueHeight = EditorGUI.GetPropertyHeight(valueElement, true);
      float height = Mathf.Max(keyHeight, valueHeight);

      Rect keyRect = new Rect(rect.x, rect.y, columnWidth, height);
      Rect valueRect = new Rect(rect.x + columnWidth + COLUMN_SPACING, rect.y, columnWidth, height);

      EditorGUI.PropertyField(keyRect, keyElement, GUIContent.none, true);
      EditorGUI.PropertyField(valueRect, valueElement, GUIContent.none, true);
    }

    private float CalculateElementHeight(SerializedProperty property, int index)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (!IsValidIndex(index, keyArray, valueArray))
        return EditorGUIUtility.singleLineHeight;

      SerializedProperty keyElement = keyArray.GetArrayElementAtIndex(index);
      SerializedProperty valueElement = valueArray.GetArrayElementAtIndex(index);

      float keyHeight = EditorGUI.GetPropertyHeight(keyElement, true);
      float valueHeight = EditorGUI.GetPropertyHeight(valueElement, true);

      return Mathf.Max(keyHeight, valueHeight) +
             EditorGUIUtility.standardVerticalSpacing * 2;
    }

    private void AddElement(SerializedProperty property)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      int index = keyArray.arraySize;

      keyArray.InsertArrayElementAtIndex(index);
      valueArray.InsertArrayElementAtIndex(index);

      // Unity copies the value of the previous element when inserting a line.
      // If the key turns out to be a duplicate — DictionaryData will silently discard the record during deserialization.
      // We ensure uniqueness by generating a placeholder key.
      SerializedProperty newKey = keyArray.GetArrayElementAtIndex(index);
      newKey.stringValue = GenerateUniqueKey(keyArray, index);

      property.serializedObject.ApplyModifiedProperties();
    }

    private string GenerateUniqueKey(SerializedProperty keyArray, int newIndex)
    {
      const string baseName = "param_";
      int counter = newIndex;

      while (true)
      {
        string candidate = baseName + counter;
        bool taken = false;

        for (int i = 0; i < newIndex; i++)
        {
          if (keyArray.GetArrayElementAtIndex(i).stringValue == candidate)
          {
            taken = true;
            break;
          }
        }

        if (!taken)
          return candidate;

        counter++;
      }
    }

    private void RemoveElement(SerializedProperty property, int index)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (!IsValidIndex(index, keyArray, valueArray))
        return;

      keyArray.DeleteArrayElementAtIndex(index);
      valueArray.DeleteArrayElementAtIndex(index);

      property.serializedObject.ApplyModifiedProperties();
    }

    private void EnsureArraySynchronization(SerializedProperty property)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (keyArray.arraySize != valueArray.arraySize)
      {
        int min = Mathf.Min(keyArray.arraySize, valueArray.arraySize);
        keyArray.arraySize = min;
        valueArray.arraySize = min;
      }
    }

    private bool IsValidIndex(int index, SerializedProperty keyArray, SerializedProperty valueArray)
    {
      return index >= 0 &&
             index < keyArray.arraySize &&
             index < valueArray.arraySize;
    }
  }
}
#endif
