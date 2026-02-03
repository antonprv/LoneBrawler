// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Tools
{
  public class QuickPrefabSwitcher : EditorWindow
  {
    private List<GameObject> prefabs = new List<GameObject>();
    private int selectedIndex = 0;
    private const int maxColumns = 4;
    const float windowWidth = 200f;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Quick Prefab Switcher")]
    public static void ShowWindow()
    {
      GetWindow(typeof(QuickPrefabSwitcher));
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
          }

          Repaint();
        }
      }

      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
      RenderPrefabsList();
      EditorGUILayout.EndScrollView();

      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("Clear List", EditorStyles.miniButtonLeft))
        ClearPrefabsList();

      if (GUILayout.Button("Add New", EditorStyles.miniButtonRight))
      {
        EditorGUIUtility.ShowObjectPicker<GameObject>(
            null,
            false,
            "t:GameObject",
            GUIUtility.GetControlID(FocusType.Passive)
        );
      }

      if (Event.current.commandName == "ObjectSelectorClosed")
      {
        var selectedObj = EditorGUIUtility.GetObjectPickerObject();
        if (selectedObj is GameObject gameObject)
          AddPrefab(gameObject);
      }

      EditorGUILayout.EndHorizontal();
    }

    void RenderPrefabsList()
    {
      int columnsCount = CalculateColumnsCount();
      float buttonWidth = Mathf.Min(windowWidth / columnsCount - 10, 80);

      for (int row = 0; row < Mathf.CeilToInt((float)prefabs.Count / columnsCount); row++)
      {
        EditorGUILayout.BeginHorizontal();
        for (int col = 0; col < columnsCount; col++)
        {
          int index = row * columnsCount + col;
          if (index >= prefabs.Count)
            break;

          Rect rect = GUILayoutUtility.GetRect(buttonWidth, 30);
          bool isSelected = (selectedIndex == index);
          Color originalColor = GUI.color;
          GUI.color = isSelected ? Color.beige : Color.white;

          if (GUI.Button(rect, prefabs[index].name))
          {
            SelectPrefab(index);
          }

          GUI.color = originalColor;
        }
        EditorGUILayout.EndHorizontal();
      }
    }

    int CalculateColumnsCount() =>
      Mathf.Clamp(Mathf.FloorToInt((float)(prefabs.Count + 1) / maxColumns) + 1, 1, maxColumns);

    void SelectPrefab(int index)
    {
      selectedIndex = index;
      Selection.activeObject = prefabs[index];
    }

    void OpenPrefabInProjectView(int index) =>
      AssetDatabase.OpenAsset(prefabs[index]);

    void AddPrefab(GameObject obj)
    {
      if (prefabs.Contains(obj)) return;
      prefabs.Add(obj);
    }

    void ClearPrefabsList()
    {
      prefabs.Clear();
      selectedIndex = 0;
    }
  }
}
