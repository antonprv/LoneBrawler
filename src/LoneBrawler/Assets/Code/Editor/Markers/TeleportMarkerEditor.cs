// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData.Configs;
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
    public static void DrawTeleportMarkerGizmo(LevelTeleportMarker teleport, GizmoType gizmoType)
    {
      Color gizmoColor = Color.coral;

      Gizmos.color = gizmoColor;
      Gizmos.DrawCube(teleport.transform.position, teleport.transform.localScale);
      Gizmos.DrawWireCube(teleport.transform.position, teleport.transform.localScale);
    }

    private void OnEnable() => _teleportMarker = (LevelTeleportMarker)target;

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUILayout.Space(_fieldSpace);
      DrawUniqueNameField();
      EditorGUILayout.Space(_fieldSpace);
      DrawLevelSelectorField();
      EditorGUILayout.Space(_fieldSpace);
      DrawEnterMarkerField();
      EditorGUILayout.Space(_fieldSpace);

      serializedObject.ApplyModifiedProperties();
    }

    private void DrawUniqueNameField()
    {
      SerializedProperty uniqueNameProp =
        serializedObject.FindProperty(nameof(_teleportMarker.UniqueName));
      uniqueNameProp.stringValue =
        EditorGUILayout.TextField("Teleport Unique Name", uniqueNameProp.stringValue);
    }

    private void DrawLevelSelectorField()
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
    private void DrawEnterMarkerField()
    {
      SerializedProperty enterMarkerProp =
        serializedObject.FindProperty(nameof(_teleportMarker.EnterMarker));

      enterMarkerProp.objectReferenceValue =
        EditorGUILayout.ObjectField(
          label: "Enter Marker",
          obj: enterMarkerProp.objectReferenceValue,
          objType: typeof(TeleportEnterMarker),
          allowSceneObjects: true
          );
    }
  }
}
