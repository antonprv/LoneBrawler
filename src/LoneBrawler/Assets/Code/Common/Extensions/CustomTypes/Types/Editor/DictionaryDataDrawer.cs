// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace Code.Common.Extensions.CustomTypes.Types.Editor
{
  /// <summary>
  /// Reusable helper for drawing DictionaryData with ReorderableList.
  /// Can be used from PropertyDrawers, custom Editors, or any EditorGUI context.
  /// </summary>
  public class DictionaryDataDrawerHelper
  {
    private const float COLUMN_SPACING = 10f;
    private const float VERTICAL_PADDING = 2f;

    private readonly Dictionary<string, ReorderableList> _cachedLists = new Dictionary<string, ReorderableList>();

    /// <summary>
    /// Draws the dictionary property at the specified position.
    /// </summary>
    public void DrawDictionary(Rect position, SerializedProperty property, GUIContent label)
    {
      EnsureArraySynchronization(property);

      ReorderableList list = GetOrCreateList(property, label);
      list.DoList(position);
    }

    /// <summary>
    /// Draws the dictionary property using automatic layout (for use in Editor windows).
    /// </summary>
    public void DrawDictionaryLayout(SerializedProperty property, GUIContent label)
    {
      EnsureArraySynchronization(property);

      ReorderableList list = GetOrCreateList(property, label);
      list.DoLayoutList();
    }

    /// <summary>
    /// Calculates the height needed to draw the dictionary property.
    /// </summary>
    public float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      ReorderableList list = GetOrCreateList(property, label);
      return list.GetHeight();
    }

    /// <summary>
    /// Clears all cached lists. Call this when disposing or when target objects change.
    /// </summary>
    public void ClearCache()
    {
      _cachedLists.Clear();
    }

    #region ReorderableList Management

    /// <summary>
    /// Gets an existing ReorderableList for the property or creates a new one.
    /// </summary>
    private ReorderableList GetOrCreateList(SerializedProperty property, GUIContent label)
    {
      string propertyKey = property.propertyPath;

      if (!_cachedLists.ContainsKey(propertyKey))
      {
        _cachedLists[propertyKey] = CreateReorderableList(property, label);
      }

      return _cachedLists[propertyKey];
    }

    /// <summary>
    /// Creates a new configured ReorderableList for dictionary editing.
    /// </summary>
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

      ConfigureListCallbacks(list, property, label);

      return list;
    }

    /// <summary>
    /// Configures all callbacks for the ReorderableList.
    /// </summary>
    private void ConfigureListCallbacks(ReorderableList list, SerializedProperty property, GUIContent label)
    {
      list.drawHeaderCallback = rect => DrawHeader(rect, label.text);
      list.elementHeightCallback = index => CalculateElementHeight(property, index);
      list.drawElementCallback = (rect, index, isActive, isFocused) =>
          DrawElement(rect, property, index);
      list.onAddCallback = _ => AddNewElement(property);
      list.onRemoveCallback = _ => RemoveElement(property, list.index);

      list.headerHeight = CalculateHeaderHeight();
    }

    #endregion

    #region Drawing Methods

    /// <summary>
    /// Draws the header with title and column labels.
    /// </summary>
    private void DrawHeader(Rect rect, string title)
    {
      DrawMainTitle(rect, title);
      DrawColumnLabels(rect);
    }

    /// <summary>
    /// Draws the main title at the top of the header.
    /// </summary>
    private void DrawMainTitle(Rect rect, string title)
    {
      Rect titleRect = new Rect(
          rect.x,
          rect.y,
          rect.width,
          EditorGUIUtility.singleLineHeight
      );

      EditorGUI.LabelField(titleRect, title, EditorStyles.boldLabel);
    }

    /// <summary>
    /// Draws "Key" and "Value" column labels.
    /// </summary>
    private void DrawColumnLabels(Rect rect)
    {
      float columnWidth = CalculateColumnWidth(rect.width);
      float labelYPosition = rect.y + EditorGUIUtility.singleLineHeight + VERTICAL_PADDING;

      // Key label
      Rect keyLabelRect = new Rect(
          rect.x,
          labelYPosition,
          columnWidth,
          EditorGUIUtility.singleLineHeight
      );
      EditorGUI.LabelField(keyLabelRect, "Key", EditorStyles.miniLabel);

      // Value label
      Rect valueLabelRect = new Rect(
          rect.x + columnWidth + COLUMN_SPACING,
          labelYPosition,
          columnWidth,
          EditorGUIUtility.singleLineHeight
      );
      EditorGUI.LabelField(valueLabelRect, "Value", EditorStyles.miniLabel);
    }

    /// <summary>
    /// Draws a single key-value pair element.
    /// </summary>
    private void DrawElement(Rect rect, SerializedProperty property, int index)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (!IsIndexValid(index, keyArray, valueArray))
        return;

      SerializedProperty keyElement = keyArray.GetArrayElementAtIndex(index);
      SerializedProperty valueElement = valueArray.GetArrayElementAtIndex(index);

      float columnWidth = CalculateColumnWidth(rect.width);
      rect.y += EditorGUIUtility.standardVerticalSpacing;

      DrawKeyField(rect, keyElement, columnWidth);
      DrawValueField(rect, valueElement, columnWidth);
    }

    /// <summary>
    /// Draws the key property field.
    /// </summary>
    private void DrawKeyField(Rect rect, SerializedProperty keyElement, float columnWidth)
    {
      float keyHeight = EditorGUI.GetPropertyHeight(keyElement, includeChildren: true);

      Rect keyRect = new Rect(
          rect.x,
          rect.y,
          columnWidth,
          keyHeight
      );

      EditorGUI.PropertyField(keyRect, keyElement, GUIContent.none, includeChildren: true);
    }

    /// <summary>
    /// Draws the value property field.
    /// </summary>
    private void DrawValueField(Rect rect, SerializedProperty valueElement, float columnWidth)
    {
      float valueHeight = EditorGUI.GetPropertyHeight(valueElement, includeChildren: true);

      Rect valueRect = new Rect(
          rect.x + columnWidth + COLUMN_SPACING,
          rect.y,
          columnWidth,
          valueHeight
      );

      EditorGUI.PropertyField(valueRect, valueElement, GUIContent.none, includeChildren: true);
    }

    #endregion

    #region Element Management

    /// <summary>
    /// Adds a new key-value pair to both arrays.
    /// </summary>
    private void AddNewElement(SerializedProperty property)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      int newIndex = keyArray.arraySize;

      keyArray.InsertArrayElementAtIndex(newIndex);
      valueArray.InsertArrayElementAtIndex(newIndex);

      ApplyPropertyChanges(property);
    }

    /// <summary>
    /// Removes a key-value pair from both arrays at the specified index.
    /// </summary>
    private void RemoveElement(SerializedProperty property, int index)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (!IsIndexValid(index, keyArray, valueArray))
        return;

      keyArray.DeleteArrayElementAtIndex(index);
      valueArray.DeleteArrayElementAtIndex(index);

      ApplyPropertyChanges(property);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Ensures key and value arrays have the same size by trimming to minimum.
    /// </summary>
    private void EnsureArraySynchronization(SerializedProperty property)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (keyArray.arraySize != valueArray.arraySize)
      {
        int syncedSize = Mathf.Min(keyArray.arraySize, valueArray.arraySize);
        keyArray.arraySize = syncedSize;
        valueArray.arraySize = syncedSize;
      }
    }

    /// <summary>
    /// Calculates the height needed for a single element.
    /// </summary>
    private float CalculateElementHeight(SerializedProperty property, int index)
    {
      SerializedProperty keyArray = property.FindPropertyRelative("keyData");
      SerializedProperty valueArray = property.FindPropertyRelative("valueData");

      if (!IsIndexValid(index, keyArray, valueArray))
        return EditorGUIUtility.singleLineHeight;

      SerializedProperty keyElement = keyArray.GetArrayElementAtIndex(index);
      SerializedProperty valueElement = valueArray.GetArrayElementAtIndex(index);

      float keyHeight = EditorGUI.GetPropertyHeight(keyElement, includeChildren: true);
      float valueHeight = EditorGUI.GetPropertyHeight(valueElement, includeChildren: true);

      return Mathf.Max(keyHeight, valueHeight) + EditorGUIUtility.standardVerticalSpacing * 2;
    }

    /// <summary>
    /// Calculates the height of the header section.
    /// </summary>
    private float CalculateHeaderHeight()
    {
      return EditorGUIUtility.singleLineHeight * 2 + VERTICAL_PADDING * 2;
    }

    /// <summary>
    /// Calculates the width of each column (Key and Value).
    /// </summary>
    private float CalculateColumnWidth(float totalWidth)
    {
      return (totalWidth - COLUMN_SPACING) / 2f;
    }

    /// <summary>
    /// Validates that the index is within bounds for both arrays.
    /// </summary>
    private bool IsIndexValid(int index, SerializedProperty keyArray, SerializedProperty valueArray)
    {
      return index >= 0 && index < keyArray.arraySize && index < valueArray.arraySize;
    }

    /// <summary>
    /// Applies and updates property modifications.
    /// </summary>
    private void ApplyPropertyChanges(SerializedProperty property)
    {
      property.serializedObject.ApplyModifiedProperties();
      property.serializedObject.Update();
    }

    #endregion
  }

  /// <summary>
  /// Custom property drawer for DictionaryData that provides a user-friendly 
  /// interface for editing key-value pairs in the Unity Inspector.
  /// </summary>
  [CustomPropertyDrawer(typeof(DictionaryData<,>))]
  public class DictionaryDataDrawer : PropertyDrawer
  {
    private readonly DictionaryDataDrawerHelper _helper = new DictionaryDataDrawerHelper();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      _helper.DrawDictionary(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return _helper.GetPropertyHeight(property, label);
    }
  }
}
#endif
