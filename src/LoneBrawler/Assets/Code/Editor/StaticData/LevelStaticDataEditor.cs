// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Linq;

using Code.Data.StaticData;
using Code.Data.StaticData.Configs;
using Code.Data.StaticData.Types;
using Code.Editor.Common;
using Code.External.Infrastructure.Unity;
using Code.Gameplay.Features.Enemies.Spawn;
using Code.Gameplay.LevelTeleport;
using Code.Gameplay.Utils;
using Code.Infrastructure.AssetManagement.Addresses;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(LevelStaticData))]
  class LevelStaticDataEditor : ManualSaveEditor
  {
    private LevelStaticData _levelStaticData;
    private GameConfig _gameConfigData;
    private float _foldoutSpaces = 8f;

    private void OnEnable() => _levelStaticData = (LevelStaticData)target;

    protected override void DrawInspector()
    {
      EditorGUILayout.Space(_foldoutSpaces);
      DrawSceneField();
      EditorGUILayout.Space(_foldoutSpaces);
      DrawSpawnersListField();
      EditorGUILayout.Space(_foldoutSpaces);
      DrawLevelTeleportsField();
      EditorGUILayout.Space(_foldoutSpaces);
      DrawPlayerStartField();
      EditorGUILayout.Space(_foldoutSpaces);
      DrawCollectAll();
      EditorGUILayout.Space(_foldoutSpaces);
    }

    private void DrawCollectAll()
    {
      if (GUILayout.Button("Collect all data"))
      {
        CollectSpawners();
        CollectTeleports();
        CollectPlayerStart();
      }
    }

    private void DrawSceneField()
    {
      SerializedProperty levelKeyProp = serializedObject.FindProperty(nameof(_levelStaticData.LevelKey));
      var sceneNames = InspectorUtils.GetAllScenes();

      int selectedIndex = Array.IndexOf(sceneNames, levelKeyProp.stringValue);

      if (selectedIndex == -1 && sceneNames.Length > 0)
      {
        selectedIndex = 0;
      }

      selectedIndex = EditorGUILayout.Popup("Scene Name:", selectedIndex, sceneNames);
      levelKeyProp.stringValue = sceneNames[selectedIndex];
    }

    private void DrawSpawnersListField()
    {
      SerializedProperty enemySpawnersProp = serializedObject.FindProperty(nameof(_levelStaticData.EnemySpawners));
      EditorGUILayout.PropertyField(enemySpawnersProp, new GUIContent("Enemy Spawners"), true);

      if (GUILayout.Button("Collect Spawners Data"))
        CollectSpawners();
    }

    private void CollectSpawners()
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

    private void DrawLevelTeleportsField()
    {
      SerializedProperty playerStartCoords = serializedObject.FindProperty(nameof(_levelStaticData.Teleports));
      EditorGUILayout.PropertyField(playerStartCoords, new GUIContent("Level Teleports"), true);

      if (GUILayout.Button("Collect Level Teleports"))
        CollectTeleports();
    }

    private void CollectTeleports()
    {
      _levelStaticData.Teleports =
        FindObjectsByType<LevelTeleportMarker>(FindObjectsSortMode.None)
        .Select(
          x => new LevelTeleportData(
            x.UniqueName,
            x.LevelKey,
            x.transform.ToCoordinates(),
            x.transform.localScale,
            x.EnterMarker.transform.ToCoordinates()
            )
          )
        .ToList();
    }

    private void DrawPlayerStartField()
    {
      SerializedProperty playerStartCoords = serializedObject.FindProperty(nameof(_levelStaticData.PlayerStartCoordinates));
      EditorGUILayout.PropertyField(playerStartCoords, new GUIContent("Player Start Coords"), true);

      if (GUILayout.Button("Collect Player Start Coords"))
        CollectPlayerStart();
    }

    private void CollectPlayerStart()
    {
      TryLoadStaticData();
      _levelStaticData.PlayerStartCoordinates =
        GameObject.FindWithTag(_gameConfigData.PlayerStartTag).transform.ToCoordinates();
    }

    private void TryLoadStaticData()
    {
      _gameConfigData = Resources.Load<GameConfig>(StaticDataAddresses.GameConfigAddress);
      if (!_gameConfigData)
      {
        Debug.LogWarning("Couldn't find GameConfig");
        return;
      }
    }
  }
}


