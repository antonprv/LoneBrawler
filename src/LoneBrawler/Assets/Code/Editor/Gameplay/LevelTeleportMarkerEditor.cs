// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Editor.Common;
using Code.Gameplay.LevelTeleport;

using UnityEditor;

namespace Code.Editor.Gameplay
{
  [CustomEditor(typeof(LevelTeleportMarker))]
  class LevelTeleportMarkerEditor : UnityEditor.Editor
  {
    private LevelTeleportMarker _teleportMarker;

    public override void OnInspectorGUI()
    {
      _teleportMarker = (LevelTeleportMarker)target;

      serializedObject.Update();

      DrawLevelField();

      serializedObject.ApplyModifiedProperties();
    }

    private void DrawLevelField()
    {
      SerializedProperty levelKeyProp = serializedObject.FindProperty(nameof(_teleportMarker.LevelKey));
      var sceneNames = InspectorUtils.GetAllScenes();

      int selectedIndex = Array.IndexOf(sceneNames, levelKeyProp.stringValue);

      if (selectedIndex == -1 && sceneNames.Length > 0)
      {
        selectedIndex = 0;
      }

      selectedIndex = EditorGUILayout.Popup("Teleport to:", selectedIndex, sceneNames);
      levelKeyProp.stringValue = sceneNames[selectedIndex];
    }
  }
}
