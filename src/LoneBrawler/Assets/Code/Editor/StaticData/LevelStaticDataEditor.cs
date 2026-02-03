// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.IO;
using System.Linq;

using Code.Data.StaticData;
using Code.Data.StaticData.Types;
using Code.Editor.Common;
using Code.Gameplay.Common;
using Code.Gameplay.Features.Enemies.Spawn;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(LevelStaticData))]
  class LevelStaticDataEditor : ManualSaveEditor
  {
    private LevelStaticData _levelStaticData;

    protected override void DrawInspector()
    {
      _levelStaticData = (LevelStaticData)target;
      DrawSceneField();
      DrawSpawnersListField();
    }

    private void DrawSpawnersListField()
    {
      SerializedProperty enemySpawnersProp = serializedObject.FindProperty(nameof(_levelStaticData.EnemySpawners));
      EditorGUILayout.PropertyField(enemySpawnersProp, new GUIContent("Enemy Spawners"), true);

      if (GUILayout.Button("Collect Spawners Data"))
      {
        _levelStaticData.EnemySpawners =
          FindObjectsByType<EnemySpawnMarker>(FindObjectsSortMode.None)
          .Select(
            x =>
            new EnemySpawnerData(
              x.GetComponent<UniqueId>().id,
              x.enemyTypeId,
              x.transform.position)
            )
          .ToList();
      }
    }

    private void DrawSceneField()
    {
      SerializedProperty levelKeyProp = serializedObject.FindProperty(nameof(_levelStaticData.LevelKey));
      var sceneNames = GetAllScenes();

      int selectedIndex = Array.IndexOf(sceneNames, levelKeyProp.stringValue);

      if (selectedIndex == -1 && sceneNames.Length > 0)
      {
        selectedIndex = 0;
      }

      selectedIndex = EditorGUILayout.Popup("Scene Name:", selectedIndex, sceneNames);
      levelKeyProp.stringValue = sceneNames[selectedIndex];
    }

    private static string[] GetAllScenes()
    {
      return Directory
        .GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
        .Select(Path.GetFileNameWithoutExtension)
        .OrderBy(n => n)
        .ToArray();
    }
  }
}


