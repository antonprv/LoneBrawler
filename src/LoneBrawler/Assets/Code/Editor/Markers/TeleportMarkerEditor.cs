// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData;
using Code.Editor.Common;
using Code.Gameplay.LevelTeleport;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Markers
{
  [CustomEditor(typeof(LevelTeleportMarker))]
  public class TeleportMarkerEditor : UnityEditor.Editor
  {
    private LevelTeleportMarker _teleportMarker;
    private float _fieldSpace = 8f;

    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void DrawTeleportTriggerGizmo(LevelTeleportMarker teleport, GizmoType gizmoType)
    {
      Color gizmoColor =
        ColorUtility.TryParseHtmlString("#7ebd18", out var c) ? c : Color.white;

      Gizmos.color = gizmoColor;
      Gizmos.DrawCube(teleport.transform.position, teleport.transform.localScale);
    }

    private void OnEnable() => _teleportMarker = (LevelTeleportMarker)target;

    public override void OnInspectorGUI()
    {
      EditorGUILayout.Space(_fieldSpace);

      serializedObject.Update();
      DrawLevelSelector();
      serializedObject.ApplyModifiedProperties();

      EditorGUILayout.Space(_fieldSpace);
    }

    private void DrawLevelSelector()
    {
      SerializedProperty levelKeyProp = serializedObject.FindProperty(nameof(_teleportMarker.LevelKey));
      var sceneNames = InspectorUtils.GetAllScenes();

      int selectedIndex = Array.IndexOf(sceneNames, levelKeyProp.stringValue);

      if (selectedIndex == -1 && sceneNames.Length > 0)
      {
        selectedIndex = 0;
      }

      selectedIndex = EditorGUILayout.Popup("Scene Name:", selectedIndex, sceneNames);
      levelKeyProp.stringValue = sceneNames[selectedIndex];
    }
  }
}
