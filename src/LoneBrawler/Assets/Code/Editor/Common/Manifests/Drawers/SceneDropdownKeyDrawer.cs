// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Editor.Common.Manifests.Interfaces;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Common.Manifests.Drawers
{
  #region Custom Key Drawer Support

  /// <summary>
  /// Custom key drawer that renders string keys as scene name dropdowns.
  /// Uses InspectorUtils.GetAllScenes() to populate the dropdown.
  /// </summary>
  public class SceneDropdownKeyDrawer : ICustomKeyDrawer
  {
    private string[] _availableScenes;
    private System.Collections.Generic.Dictionary<string, int> _sceneIndexCache
        = new System.Collections.Generic.Dictionary<string, int>();

    public SceneDropdownKeyDrawer()
    {
      RefreshSceneList();
    }

    public void ClearCache()
    {
      _sceneIndexCache.Clear();
    }

    public void DrawDictionaryWithCustomKeys(SerializedProperty property, GUIContent label)
    {
      EnsureArraySynchronization(property);

      var keyArray = property.FindPropertyRelative("keyData");
      var valueArray = property.FindPropertyRelative("valueData");

      // Header
      EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
      EditorGUILayout.Space(5);

      // Refresh button
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField($"Available Scenes: {_availableScenes.Length}", EditorStyles.miniLabel);
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Refresh Scene List", GUILayout.Width(130), GUILayout.Height(22)))
      {
        RefreshSceneList();
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space(5);

      // Draw entries
      for (int i = keyArray.arraySize - 1; i >= 0; i--)
      {
        if (DrawEntry(keyArray, valueArray, i))
        {
          // Entry was deleted, apply changes
          property.serializedObject.ApplyModifiedProperties();
        }
      }

      EditorGUILayout.Space(5);

      // Add button
      GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
      if (GUILayout.Button("Add Entry", GUILayout.Height(28)))
      {
        keyArray.InsertArrayElementAtIndex(keyArray.arraySize);
        valueArray.InsertArrayElementAtIndex(valueArray.arraySize);

        // Set default value to first scene
        if (_availableScenes.Length > 0)
        {
          var newKeyProp = keyArray.GetArrayElementAtIndex(keyArray.arraySize - 1);
          newKeyProp.stringValue = _availableScenes[0];
        }

        property.serializedObject.ApplyModifiedProperties();
      }
      GUI.backgroundColor = Color.white;
    }

    private bool DrawEntry(SerializedProperty keyArray, SerializedProperty valueArray, int index)
    {
      EditorGUILayout.BeginVertical(EditorStyles.helpBox);

      EditorGUILayout.BeginHorizontal();

      // Scene dropdown for key
      var keyProperty = keyArray.GetArrayElementAtIndex(index);
      string currentKey = keyProperty.stringValue;
      int currentIndex = GetSceneIndex(currentKey);

      EditorGUILayout.BeginVertical();
      int newIndex = EditorGUILayout.Popup("Scene", currentIndex, _availableScenes);
      if (newIndex != currentIndex && newIndex >= 0 && newIndex < _availableScenes.Length)
      {
        keyProperty.stringValue = _availableScenes[newIndex];
      }

      // Value property
      var valueProperty = valueArray.GetArrayElementAtIndex(index);
      EditorGUILayout.PropertyField(valueProperty, new GUIContent("Level Data"), true);
      EditorGUILayout.EndVertical();

      // Remove button
      GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
      bool removed = false;
      if (GUILayout.Button("×", GUILayout.Width(30), GUILayout.Height(40)))
      {
        if (EditorUtility.DisplayDialog(
            "Remove Entry",
            $"Remove level entry for scene '{currentKey}'?",
            "Remove",
            "Cancel"))
        {
          keyArray.DeleteArrayElementAtIndex(index);
          valueArray.DeleteArrayElementAtIndex(index);
          removed = true;
        }
      }
      GUI.backgroundColor = Color.white;

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.EndVertical();
      EditorGUILayout.Space(3);

      return removed;
    }

    private void RefreshSceneList()
    {
      _availableScenes = InspectorUtils.GetAllScenes();
      _sceneIndexCache.Clear();

      for (int i = 0; i < _availableScenes.Length; i++)
      {
        _sceneIndexCache[_availableScenes[i]] = i;
      }
    }

    private int GetSceneIndex(string sceneName)
    {
      if (string.IsNullOrEmpty(sceneName))
        return 0;

      if (_sceneIndexCache.TryGetValue(sceneName, out int index))
        return index;

      return 0;
    }

    private void EnsureArraySynchronization(SerializedProperty property)
    {
      var keyArray = property.FindPropertyRelative("keyData");
      var valueArray = property.FindPropertyRelative("valueData");

      if (keyArray.arraySize != valueArray.arraySize)
      {
        int syncedSize = Mathf.Min(keyArray.arraySize, valueArray.arraySize);
        keyArray.arraySize = syncedSize;
        valueArray.arraySize = syncedSize;
      }
    }
  }

  #endregion
}
#endif
