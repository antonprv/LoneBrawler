// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Linq;

using Code.Common.Extensions.CustomTypes.Types;
using Code.Data.StaticData;
using Code.Gameplay.LevelTeleport;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace Code.Editor.Markers
{
  [CustomEditor(typeof(LevelTeleportMarker))]
  public class TeleportMarkerEditor : UnityEditor.Editor
  {
    #region Constants

    private const string TELEPORT_DATA_PATH = "StaticData/TeleportListData";
    private const float FIELD_SPACING = 8f;

    #endregion

    #region Fields

    private LevelTeleportMarker _teleportMarker;
    private TeleportListData _teleportData;
    private string[] _availableKeys;
    private int _selectedKeyIndex;
    private string _currentSceneName;

    #endregion

    #region Unity Callbacks

    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void DrawTeleportMarkerGizmo(LevelTeleportMarker teleport, GizmoType gizmoType)
    {
      Color gizmoColor = Color.coral;

      Gizmos.color = gizmoColor;
      Gizmos.DrawCube(teleport.transform.position, teleport.transform.localScale);
      Gizmos.DrawWireCube(teleport.transform.position, teleport.transform.localScale);
    }

    private void OnEnable()
    {
      _teleportMarker = (LevelTeleportMarker)target;
      _currentSceneName = EditorSceneManager.GetActiveScene().name;

      LoadTeleportData();
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      if (_teleportData == null)
      {
        DrawNoDataWarning();
        serializedObject.ApplyModifiedProperties();
        return;
      }

      DrawInspectorContent();

      serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Data Loading

    private void LoadTeleportData()
    {
      _teleportData = Resources.Load<TeleportListData>(TELEPORT_DATA_PATH);

      if (_teleportData == null)
      {
        Debug.LogWarning($"TeleportListData not found at Resources/{TELEPORT_DATA_PATH}");
        return;
      }

      RefreshAvailableKeys();
    }

    private void RefreshAvailableKeys()
    {
      if (_teleportData?.TeleportList == null)
      {
        _availableKeys = new string[0];
        return;
      }

      _availableKeys = _teleportData.TeleportList.Keys.ToArray();
      _selectedKeyIndex = FindKeyIndex(_teleportMarker.UniqueName);
    }

    private int FindKeyIndex(string uniqueName)
    {
      int index = Array.IndexOf(_availableKeys, uniqueName);
      return index == -1 && _availableKeys.Length > 0 ? 0 : index;
    }

    #endregion

    #region Inspector Drawing

    private void DrawInspectorContent()
    {
      EditorGUILayout.Space(FIELD_SPACING);
      DrawUniqueNameField();

      EditorGUILayout.Space(FIELD_SPACING);
      DrawSameLevelToggle();

      EditorGUILayout.Space(FIELD_SPACING);
      DrawDestinationSceneField();

      EditorGUILayout.Space(FIELD_SPACING);
      DrawValidationInfo();

      EditorGUILayout.Space(FIELD_SPACING);
      DrawEnterMarkerField();

      EditorGUILayout.Space(FIELD_SPACING);
    }

    private void DrawNoDataWarning()
    {
      EditorGUILayout.HelpBox(
        $"TeleportListData not found at Resources/{TELEPORT_DATA_PATH}\n" +
        "Please create it or check the path.",
        MessageType.Error);

      if (GUILayout.Button("Retry Load"))
      {
        LoadTeleportData();
      }
    }

    #endregion

    #region Unique Name Field

    private void DrawUniqueNameField()
    {
      SerializedProperty uniqueNameProperty =
        serializedObject.FindProperty(nameof(_teleportMarker.UniqueName));

      if (!HasAvailableTeleports())
      {
        DrawNoTeleportsAvailableWarning();
        return;
      }

      DrawUniqueNameDropdown(uniqueNameProperty);
    }

    private bool HasAvailableTeleports()
    {
      return _availableKeys.Length > 0;
    }

    private void DrawNoTeleportsAvailableWarning()
    {
      EditorGUILayout.HelpBox(
        "No teleport configurations available in TeleportListData.\n" +
        "Please add teleports in the TeleportListData asset first.",
        MessageType.Warning);

      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.TextField("Teleport Unique Name", "No teleports available");
      EditorGUI.EndDisabledGroup();
    }

    private void DrawUniqueNameDropdown(SerializedProperty uniqueNameProperty)
    {
      EditorGUI.BeginChangeCheck();

      _selectedKeyIndex = EditorGUILayout.Popup(
        "Teleport Unique Name",
        _selectedKeyIndex,
        _availableKeys);

      if (EditorGUI.EndChangeCheck())
      {
        uniqueNameProperty.stringValue = _availableKeys[_selectedKeyIndex];
      }
    }

    #endregion

    #region Same Level Toggle

    private void DrawSameLevelToggle()
    {
      SerializedProperty sameLevelProperty =
        serializedObject.FindProperty(nameof(_teleportMarker.TeleportsToSameLevel));

      EditorGUI.BeginChangeCheck();

      sameLevelProperty.boolValue = EditorGUILayout.Toggle(
        new GUIContent(
          "Teleports To Same Level",
          "Enable if this teleport is within the same level (disables cross-level validation)"),
        sameLevelProperty.boolValue);

      if (EditorGUI.EndChangeCheck())
      {
        ClearDestinationScene();
      }
    }

    private void ClearDestinationScene()
    {
      serializedObject.FindProperty(nameof(_teleportMarker.LevelKey)).stringValue = "";
    }

    #endregion

    #region Destination Scene Field

    private void DrawDestinationSceneField()
    {
      SerializedProperty destinationSceneProperty =
        serializedObject.FindProperty(nameof(_teleportMarker.LevelKey));

      ApplyPendingChanges();

      if (!HasUniqueNameSelected())
      {
        DrawSelectUniqueNameFirstMessage();
        return;
      }

      if (!IsTeleportConfigurationValid())
      {
        DrawTeleportNotFoundError();
        return;
      }

      DrawDestinationSceneDropdown(destinationSceneProperty);
    }

    private void ApplyPendingChanges()
    {
      serializedObject.ApplyModifiedProperties();
      serializedObject.Update();
    }

    private bool HasUniqueNameSelected()
    {
      return !string.IsNullOrEmpty(_teleportMarker.UniqueName);
    }

    private void DrawSelectUniqueNameFirstMessage()
    {
      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.TextField("Destination Scene", "Select Unique Name first");
      EditorGUI.EndDisabledGroup();
    }

    private bool IsTeleportConfigurationValid()
    {
      return _teleportData.TeleportList.ContainsKey(_teleportMarker.UniqueName);
    }

    private void DrawTeleportNotFoundError()
    {
      EditorGUILayout.HelpBox(
        $"Teleport '{_teleportMarker.UniqueName}' not found in TeleportListData.",
        MessageType.Error);
    }

    private void DrawDestinationSceneDropdown(SerializedProperty destinationSceneProperty)
    {
      PairData<string, string> teleportRoute = GetTeleportRoute();
      string[] availableDestinations = GetAvailableDestinations(teleportRoute);

      if (!HasValidDestinations(availableDestinations))
      {
        DrawNoValidDestinationsError(teleportRoute);
        return;
      }

      DrawSceneDropdown(destinationSceneProperty, availableDestinations);
      DrawSceneContextInfo(teleportRoute);
    }

    private PairData<string, string> GetTeleportRoute()
    {
      return _teleportData.TeleportList[_teleportMarker.UniqueName];
    }

    private string[] GetAvailableDestinations(PairData<string, string> teleportRoute)
    {
      if (_teleportMarker.TeleportsToSameLevel)
        return GetSameLevelDestinations(teleportRoute);

      return GetCrossLevelDestinations(teleportRoute);
    }

    private string[] GetSameLevelDestinations(PairData<string, string> teleportRoute)
    {
      List<string> destinations = new List<string> { teleportRoute.Key };

      if (teleportRoute.Key != teleportRoute.Value)
      {
        destinations.Add(teleportRoute.Value);
      }

      return destinations.ToArray();
    }

    private string[] GetCrossLevelDestinations(PairData<string, string> teleportRoute)
    {
      List<string> destinations = new List<string>();
      string occupiedLevel = FindOccupiedLevel(_teleportMarker.UniqueName);

      AddSceneIfValid(destinations, teleportRoute.Key, occupiedLevel);
      AddSceneIfValid(destinations, teleportRoute.Value, occupiedLevel);

      return destinations.ToArray();
    }

    private void AddSceneIfValid(List<string> destinations, string sceneName, string occupiedLevel)
    {
      bool isCurrentScene = sceneName == _currentSceneName;
      bool isDuplicate = destinations.Contains(sceneName);
      bool isOccupied = sceneName == occupiedLevel;

      if (!isCurrentScene && !isDuplicate && !isOccupied)
      {
        destinations.Add(sceneName);
      }
    }

    private bool HasValidDestinations(string[] destinations)
    {
      return destinations.Length > 0;
    }

    private void DrawNoValidDestinationsError(PairData<string, string> teleportRoute)
    {
      string occupiedLevel = FindOccupiedLevel(_teleportMarker.UniqueName);

      EditorGUILayout.HelpBox(
        $"Cannot configure teleport:\n" +
        $"• Current scene: {_currentSceneName}\n" +
        $"• Route: {teleportRoute.Key} ↔ {teleportRoute.Value}\n" +
        $"• Occupied: {occupiedLevel ?? "None"}\n\n" +
        $"No valid destination available. Try enabling 'Teleports To Same Level' or check other markers.",
        MessageType.Error);

      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.TextField("Destination Scene", "No valid destinations");
      EditorGUI.EndDisabledGroup();
    }

    private void DrawSceneDropdown(SerializedProperty destinationSceneProperty, string[] availableDestinations)
    {
      int currentIndex = Array.IndexOf(availableDestinations, destinationSceneProperty.stringValue);

      if (currentIndex == -1)
      {
        currentIndex = 0;
        destinationSceneProperty.stringValue = availableDestinations[0];
      }

      EditorGUI.BeginChangeCheck();

      currentIndex = EditorGUILayout.Popup("Destination Scene", currentIndex, availableDestinations);

      if (EditorGUI.EndChangeCheck())
      {
        destinationSceneProperty.stringValue = availableDestinations[currentIndex];
      }
    }

    private void DrawSceneContextInfo(PairData<string, string> teleportRoute)
    {
      EditorGUILayout.LabelField("Current Scene:", _currentSceneName, EditorStyles.miniLabel);
      EditorGUILayout.LabelField("Teleport Route:", $"{teleportRoute.Key} ↔ {teleportRoute.Value}", EditorStyles.miniLabel);
    }

    #endregion

    #region Validation Info

    private void DrawValidationInfo()
    {
      if (!CanShowValidationInfo())
        return;

      if (_teleportMarker.TeleportsToSameLevel)
      {
        DrawSameLevelModeInfo();
        return;
      }

      DrawCrossLevelValidationInfo();
    }

    private bool CanShowValidationInfo()
    {
      return !string.IsNullOrEmpty(_teleportMarker.UniqueName) &&
             !string.IsNullOrEmpty(_teleportMarker.LevelKey);
    }

    private void DrawSameLevelModeInfo()
    {
      EditorGUILayout.HelpBox(
        "Same-level teleport mode: Cross-level validation is disabled.",
        MessageType.Info);
    }

    private void DrawCrossLevelValidationInfo()
    {
      string occupiedLevel = FindOccupiedLevel(_teleportMarker.UniqueName);

      if (IsLevelOccupiedByOther(occupiedLevel))
      {
        DrawLevelOccupiedInfo(occupiedLevel);
      }
      else if (HasConflictingMarker())
      {
        DrawConflictError();
      }
    }

    private bool IsLevelOccupiedByOther(string occupiedLevel)
    {
      return occupiedLevel != null && occupiedLevel != _teleportMarker.LevelKey;
    }

    private void DrawLevelOccupiedInfo(string occupiedLevel)
    {
      string availableLevel = GetAlternativeLevel(_teleportMarker.UniqueName);

      EditorGUILayout.HelpBox(
        $"Level '{occupiedLevel}' is already occupied by another marker with the same UniqueName.\n" +
        $"Available option: '{availableLevel}'",
        MessageType.Info);
    }

    private bool HasConflictingMarker()
    {
      string occupiedLevel = FindOccupiedLevel(_teleportMarker.UniqueName);
      return occupiedLevel == _teleportMarker.LevelKey;
    }

    private void DrawConflictError()
    {
      LevelTeleportMarker conflictingMarker = FindConflictingMarker();

      if (conflictingMarker != null)
      {
        EditorGUILayout.HelpBox(
          $"⚠️ CONFLICT: Another marker '{conflictingMarker.name}' already uses this UniqueName and LevelKey combination!",
          MessageType.Error);

        if (GUILayout.Button("Select Conflicting Marker"))
        {
          SelectAndPingMarker(conflictingMarker);
        }
      }
    }

    private void SelectAndPingMarker(LevelTeleportMarker marker)
    {
      Selection.activeGameObject = marker.gameObject;
      EditorGUIUtility.PingObject(marker.gameObject);
    }

    #endregion

    #region Enter Marker Field

    private void DrawEnterMarkerField()
    {
      SerializedProperty enterMarkerProperty =
        serializedObject.FindProperty(nameof(_teleportMarker.EnterMarker));

      enterMarkerProperty.objectReferenceValue =
        EditorGUILayout.ObjectField(
          label: "Enter Marker",
          obj: enterMarkerProperty.objectReferenceValue,
          objType: typeof(TeleportEnterMarker),
          allowSceneObjects: true);
    }

    #endregion

    #region Validation Helpers

    private string FindOccupiedLevel(string uniqueName)
    {
      if (!_teleportData.TeleportList.ContainsKey(uniqueName))
        return null;

      LevelTeleportMarker[] allMarkers = FindAllTeleportMarkers();

      foreach (LevelTeleportMarker marker in allMarkers)
      {
        if (IsOccupyingMarker(marker, uniqueName))
        {
          return marker.LevelKey;
        }
      }

      return null;
    }

    private LevelTeleportMarker[] FindAllTeleportMarkers()
    {
      List<LevelTeleportMarker> markers = new List<LevelTeleportMarker>();

      markers.AddRange(FindObjectsByType<LevelTeleportMarker>(FindObjectsSortMode.None));
      markers.AddRange(FindMarkersInAllLoadedScenes());

      return markers.ToArray();
    }

    private IEnumerable<LevelTeleportMarker> FindMarkersInAllLoadedScenes()
    {
      List<LevelTeleportMarker> markers = new List<LevelTeleportMarker>();
      int sceneCount = EditorSceneManager.sceneCount;

      for (int i = 0; i < sceneCount; i++)
      {
        var scene = EditorSceneManager.GetSceneAt(i);
        if (scene.isLoaded)
        {
          markers.AddRange(FindMarkersInScene(scene));
        }
      }

      return markers;
    }

    private IEnumerable<LevelTeleportMarker> FindMarkersInScene(UnityEngine.SceneManagement.Scene scene)
    {
      List<LevelTeleportMarker> markers = new List<LevelTeleportMarker>();
      GameObject[] rootObjects = scene.GetRootGameObjects();

      foreach (GameObject root in rootObjects)
      {
        markers.AddRange(root.GetComponentsInChildren<LevelTeleportMarker>(true));
      }

      return markers;
    }

    private bool IsOccupyingMarker(LevelTeleportMarker marker, string uniqueName)
    {
      bool isSelf = marker == _teleportMarker;
      bool isSameLevel = marker.TeleportsToSameLevel;
      bool hasMatchingName = marker.UniqueName == uniqueName;
      bool hasLevelKey = !string.IsNullOrEmpty(marker.LevelKey);

      return !isSelf && !isSameLevel && hasMatchingName && hasLevelKey;
    }

    private string GetAlternativeLevel(string uniqueName)
    {
      if (!_teleportData.TeleportList.ContainsKey(uniqueName))
        return "";

      PairData<string, string> teleportRoute = _teleportData.TeleportList[uniqueName];
      string occupiedLevel = FindOccupiedLevel(uniqueName);

      if (occupiedLevel == null)
        return teleportRoute.Key;

      return occupiedLevel == teleportRoute.Key ? teleportRoute.Value : teleportRoute.Key;
    }

    private LevelTeleportMarker FindConflictingMarker()
    {
      LevelTeleportMarker[] allMarkers =
        FindObjectsByType<LevelTeleportMarker>(FindObjectsSortMode.None);

      foreach (LevelTeleportMarker marker in allMarkers)
      {
        if (IsConflictingMarker(marker))
        {
          return marker;
        }
      }

      return null;
    }

    private bool IsConflictingMarker(LevelTeleportMarker marker)
    {
      bool isSelf = marker == _teleportMarker;
      bool hasMatchingName = marker.UniqueName == _teleportMarker.UniqueName;
      bool hasMatchingLevel = marker.LevelKey == _teleportMarker.LevelKey;
      bool isSameLevel = marker.TeleportsToSameLevel;

      return !isSelf && hasMatchingName && hasMatchingLevel && !isSameLevel;
    }

    #endregion
  }
}
