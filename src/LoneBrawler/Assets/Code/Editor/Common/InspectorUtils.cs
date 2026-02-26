// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Common
{
  public class InspectorUtils : UnityEditor.Editor
  {
    public static Color fleaBellyColor = new Color(78f, 22f, 9f);

    public static void DrawFoldout(
      SerializedObject serializedObject,
      string title,
      ref bool state,
      string[] fieldNames)
    {
      state = EditorGUILayout.BeginFoldoutHeaderGroup(state, title);

      if (state)
      {
        foreach (string field in fieldNames)
        {
          SerializedProperty property = serializedObject.FindProperty(field);
          EditorGUILayout.PropertyField(property, true);
        }
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public static string[] GetAllScenes()
    {
      return Directory
        .GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
        .Select(Path.GetFileNameWithoutExtension)
        .OrderBy(n => n)
        .ToArray();
    }

    /// <summary>
    /// Draws an enum popup excluding specified values
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="property">SerializedProperty containing enum value</param>
    /// <param name="label">Label for display</param>
    /// <param name="excludedValues">Enum values to hide</param>
    public static void DrawFilteredEnumPopup<TEnum>(
      SerializedProperty property,
      GUIContent label,
      params TEnum[] excludedValues) where TEnum : Enum
    {
      // Get current value
      TEnum currentValue = (TEnum)Enum.ToObject(typeof(TEnum), property.enumValueIndex);

      // Get all enum values except those in excludedValues
      var availableValues = Enum.GetValues(typeof(TEnum))
        .Cast<TEnum>()
        .Where(value => !excludedValues.Contains(value))
        .ToArray();

      // If there are no available values, show a warning
      if (availableValues.Length == 0)
      {
        EditorGUILayout.HelpBox(
          $"No available values for {typeof(TEnum).Name}",
          MessageType.Warning);
        return;
      }

      // Get names for display
      var displayNames = availableValues
        .Select(value => value.ToString())
        .ToArray();

      // Find index of current value in filtered array
      int selectedIndex = Array.IndexOf(availableValues, currentValue);

      // If current value is excluded, set first available
      if (selectedIndex < 0)
      {
        selectedIndex = 0;
        currentValue = availableValues[0];
        property.enumValueIndex = Convert.ToInt32(currentValue);
      }

      // Draw popup
      EditorGUI.BeginChangeCheck();
      int newIndex = EditorGUILayout.Popup(label, selectedIndex, displayNames);

      if (EditorGUI.EndChangeCheck() && newIndex >= 0 && newIndex < availableValues.Length)
      {
        TEnum newValue = availableValues[newIndex];
        property.enumValueIndex = Convert.ToInt32(newValue);
      }
    }

    /// <summary>
    /// Overload with string label instead of GUIContent
    /// </summary>
    public static void DrawFilteredEnumPopup<TEnum>(
      SerializedProperty property,
      string label,
      params TEnum[] excludedValues) where TEnum : Enum
    {
      DrawFilteredEnumPopup(property, new GUIContent(label), excludedValues);
    }
  }
}
