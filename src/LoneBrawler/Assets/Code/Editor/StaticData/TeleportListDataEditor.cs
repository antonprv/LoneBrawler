// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Linq;

using Code.Common.Extensions.CustomTypes.Types;
using Code.Data.StaticData;
using Code.Editor.Common;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(TeleportListData))]
  public class TeleportListDataEditor : ManualSaveEditor
  {
    private TeleportListData _teleportListData;
    private string[] _availableScenes;
    private Dictionary<string, int> _sceneIndexCache = new Dictionary<string, int>();

    private string _newKey = "";
    private int _newFromSceneIndex = 0;
    private int _newToSceneIndex = 0;

    private void OnEnable()
    {
      _teleportListData = (TeleportListData)target;
      RefreshSceneList();
    }

    protected override void DrawInspector()
    {
      EditorGUILayout.Space(10);
      EditorGUILayout.LabelField("Teleport List Configuration", EditorStyles.boldLabel);
      EditorGUILayout.Space(5);

      DrawRefreshButton();
      EditorGUILayout.Space(10);

      DrawAddNewTeleport();
      EditorGUILayout.Space(15);

      DrawTeleportList();
    }

    private void RefreshSceneList()
    {
      _availableScenes = InspectorUtils.GetAllScenes();
      _sceneIndexCache.Clear();

      // Rebuild cache
      for (int i = 0; i < _availableScenes.Length; i++)
      {
        _sceneIndexCache[_availableScenes[i]] = i;
      }
    }

    private void DrawRefreshButton()
    {
      if (GUILayout.Button("Refresh Scene List", GUILayout.Height(25)))
      {
        RefreshSceneList();
      }

      EditorGUILayout.LabelField($"Available Scenes: {_availableScenes.Length}", EditorStyles.miniLabel);
    }

    private void DrawAddNewTeleport()
    {
      EditorGUILayout.LabelField("Add New Teleport", EditorStyles.boldLabel);

      EditorGUILayout.BeginVertical(EditorStyles.helpBox);

      _newKey = EditorGUILayout.TextField("Teleport Unique ID", _newKey);

      EditorGUILayout.Space(5);

      _newFromSceneIndex = EditorGUILayout.Popup("From Scene", _newFromSceneIndex, _availableScenes);
      _newToSceneIndex = EditorGUILayout.Popup("To Scene", _newToSceneIndex, _availableScenes);

      EditorGUILayout.Space(5);

      GUI.backgroundColor = Color.beige;
      bool canAdd = !string.IsNullOrWhiteSpace(_newKey) && !_teleportListData.TeleportList.ContainsKey(_newKey);

      using (new EditorGUI.DisabledScope(!canAdd))
      {
        if (GUILayout.Button("Add Teleport", GUILayout.Height(25)))
        {
          string fromScene = _availableScenes[_newFromSceneIndex];
          string toScene = _availableScenes[_newToSceneIndex];

          _teleportListData.TeleportList.Add(
            _newKey,
            new PairData<string, string>(fromScene, toScene)
          );

          _newKey = "";
          GUI.FocusControl(null);
        }
      }

      GUI.backgroundColor = Color.white;

      if (!canAdd && !string.IsNullOrWhiteSpace(_newKey))
      {
        EditorGUILayout.HelpBox("Teleport ID already exists!", MessageType.Warning);
      }

      EditorGUILayout.EndVertical();
    }

    private void DrawTeleportList()
    {
      EditorGUILayout.LabelField($"Existing Teleports ({_teleportListData.TeleportList.Count})", EditorStyles.boldLabel);

      if (_teleportListData.TeleportList.Count == 0)
      {
        EditorGUILayout.HelpBox("No teleports configured yet. Add one above!", MessageType.Info);
        return;
      }

      EditorGUILayout.Space(5);

      // Convert to list for iteration (to avoid modification during enumeration)
      var teleportKeys = _teleportListData.TeleportList.Keys.ToList();
      string keyToRemove = null;

      foreach (string key in teleportKeys)
      {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(key, EditorStyles.boldLabel, GUILayout.Width(150));

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("×", GUILayout.Width(25), GUILayout.Height(20)))
        {
          if (EditorUtility.DisplayDialog(
            "Remove Teleport",
            $"Are you sure you want to remove teleport '{key}'?",
            "Yes",
            "No"))
          {
            keyToRemove = key;
          }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(3);

        var pairData = _teleportListData.TeleportList[key];

        // From Scene Dropdown
        int fromIndex = GetSceneIndex(pairData.Key);
        int newFromIndex = EditorGUILayout.Popup("From Scene", fromIndex, _availableScenes);

        if (newFromIndex != fromIndex)
        {
          pairData.Key = _availableScenes[newFromIndex];
        }

        // To Scene Dropdown
        int toIndex = GetSceneIndex(pairData.Value);
        int newToIndex = EditorGUILayout.Popup("To Scene", toIndex, _availableScenes);

        if (newToIndex != toIndex)
        {
          pairData.Value = _availableScenes[newToIndex];
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
      }

      // Remove after iteration to avoid collection modification
      if (keyToRemove != null)
      {
        _teleportListData.TeleportList.Remove(keyToRemove);
      }
    }

    private int GetSceneIndex(string sceneName)
    {
      if (string.IsNullOrEmpty(sceneName))
        return 0;

      if (_sceneIndexCache.TryGetValue(sceneName, out int index))
        return index;

      // If scene not found in cache (might be deleted), return 0
      return 0;
    }
  }
}
