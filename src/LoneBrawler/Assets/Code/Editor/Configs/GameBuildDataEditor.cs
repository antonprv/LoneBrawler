// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Linq;

using Code.Data.StaticData.Configs;
using Code.Data.StaticData.Configs.BuildConfig;
using Code.Editor.Common;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Configs
{
  [CustomEditor(typeof(GameBuildData))]
  public class GameBuildDataEditor : ManualSaveEditor
  {
    protected override void DrawInspector()
    {
      DrawDefaultInspectorWithManualSave();
    }
  }

  // PropertyDrawer for NoNone
  [CustomPropertyDrawer(typeof(NoNoneAttribute))]
  public class NoNoneDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (property.propertyType == SerializedPropertyType.Enum)
      {
        // Get all values except None
        var enumNames = property.enumNames;
        var enumValues = System.Enum.GetValues(typeof(BuildConfiguration));
        int[] allowedIndices = Enumerable.Range(0, enumNames.Length)
            .Where(i => (BuildConfiguration)enumValues.GetValue(i) != BuildConfiguration.None)
            .ToArray();

        // Build array with names
        string[] allowedNames = allowedIndices.Select(i => enumNames[i]).ToArray();
        int currentIndex = System.Array.IndexOf(allowedIndices, property.enumValueIndex);

        // Output popup
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, allowedNames);
        if (newIndex >= 0 && newIndex < allowedIndices.Length)
          property.enumValueIndex = allowedIndices[newIndex];
      }
      else
      {
        EditorGUI.PropertyField(position, property, label);
      }
    }
  }
}
