// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Tools
{
  public class QuickSwitcher : EditorWindow
  {
    private List<GameObject> _prefabs = new List<GameObject>();
    private List<ScriptableObject> _scriptableObjects = new List<ScriptableObject>();

    private int _selectedIndex = 0;
    private const int _maxColumns = 3;
    private float _spacingBetweenButtons = 10f;
    private const float _windowWidth = 200f;
    private Vector2 _scrollPosition = Vector2.zero;

    [MenuItem("Window/Quick Switcher")]
    public static void ShowWindow()
    {
      GetWindow(typeof(QuickSwitcher));
    }

    void OnGUI()
    {
      Event currentEvent = Event.current;

      if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
      {
        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (currentEvent.type == EventType.DragPerform)
        {
          foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
          {
            if (draggedObject is GameObject gameObject)
              AddPrefab(gameObject);

            if (draggedObject is ScriptableObject scriptableObject)
              AddScriptableObject(scriptableObject);
          }

          Repaint();
        }
      }

      _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

      RenderPrefabsList();
      RenderScriptableObjectsList();

      EditorGUILayout.EndScrollView();

      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("Clear Lists", EditorStyles.miniButtonLeft))
        ClearLists();

      if (GUILayout.Button("Add New", EditorStyles.miniButtonRight))
      {
        EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:GameObject", GUIUtility.GetControlID(FocusType.Passive));
      }

      if (Event.current.commandName == "ObjectSelectorClosed")
      {
        var selectedObj = EditorGUIUtility.GetObjectPickerObject();
        if (selectedObj != null && selectedObj is GameObject gameObject)
          AddPrefab(gameObject);
      }

      EditorGUILayout.EndHorizontal();
    }

    private void AddScriptableObject(ScriptableObject scriptableObject)
    {
      if (!_scriptableObjects.Contains(scriptableObject))
        _scriptableObjects.Add(scriptableObject);
    }

    void ClearLists()
    {
      _prefabs.Clear();
      _scriptableObjects.Clear();
      _selectedIndex = 0;
    }

    void RenderPrefabsList()
    {
      if (_prefabs.Count > 0)
      {
        DrawHeader("Prefabs");
        int columnsCount = CalculateColumnsCount();
        float totalSpacing = (columnsCount - 1) * _spacingBetweenButtons;
        float availableSpaceForButtons = _windowWidth - totalSpacing;
        float buttonWidth = Mathf.Min(availableSpaceForButtons / columnsCount, 80);

        for (int row = 0; row < Mathf.CeilToInt((float)_prefabs.Count / columnsCount); row++)
        {
          EditorGUILayout.BeginHorizontal();
          for (int col = 0; col < columnsCount; col++)
          {
            int index = row * columnsCount + col;
            if (index >= _prefabs.Count)
              break;

            Rect rect = GUILayoutUtility.GetRect(buttonWidth, 30);
            bool isSelected = (_selectedIndex == index);
            Color originalColor = GUI.color;
            GUI.color = isSelected ? Color.beige : Color.white;

            if (GUI.Button(rect, _prefabs[index].name))
            {
              SelectPrefab(index);
            }

            GUI.color = originalColor;
          }
          EditorGUILayout.EndHorizontal();
        }
      }
    }

    void RenderScriptableObjectsList()
    {
      if (_scriptableObjects.Count > 0)
      {
        DrawHeader("Scriptable Objects");
        int columnsCount = CalculateColumnsCount();
        float totalSpacing = (columnsCount - 1) * _spacingBetweenButtons;
        float availableSpaceForButtons = _windowWidth - totalSpacing;
        float buttonWidth = Mathf.Min(availableSpaceForButtons / columnsCount, 80);

        for (int row = 0; row < Mathf.CeilToInt((float)_scriptableObjects.Count / columnsCount); row++)
        {
          EditorGUILayout.BeginHorizontal();
          for (int col = 0; col < columnsCount; col++)
          {
            int index = row * columnsCount + col;
            if (index >= _scriptableObjects.Count)
              break;

            Rect rect = GUILayoutUtility.GetRect(buttonWidth, 30);
            bool isSelected = (_selectedIndex == index + _prefabs.Count);
            Color originalColor = GUI.color;
            GUI.color = isSelected ? Color.beige : Color.white;

            if (GUI.Button(rect, _scriptableObjects[index].name))
            {
              SelectScriptableObject(index);
            }

            GUI.color = originalColor;
          }
          EditorGUILayout.EndHorizontal();
        }
      }
    }

    void SelectPrefab(int index)
    {
      _selectedIndex = index;
      Selection.activeObject = _prefabs[index];
    }

    void SelectScriptableObject(int index)
    {
      _selectedIndex = index + _prefabs.Count;
      Selection.activeObject = _scriptableObjects[index];
    }

    int CalculateColumnsCount() =>
      Mathf.Clamp(
        Mathf.FloorToInt(
          (float)(_prefabs.Count + _scriptableObjects.Count + 1) / _maxColumns) + 1,
        1,
        _maxColumns
        );

    void AddPrefab(GameObject obj)
    {
      if (!_prefabs.Contains(obj))
        _prefabs.Add(obj);
    }

    void DrawHeader(string title)
    {
      EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
    }
  }
}
