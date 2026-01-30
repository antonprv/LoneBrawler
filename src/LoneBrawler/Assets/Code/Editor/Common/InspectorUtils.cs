// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEditor;

namespace Code.Editor.Common
{
  public class InspectorUtils : UnityEditor.Editor
  {
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
  }
}
