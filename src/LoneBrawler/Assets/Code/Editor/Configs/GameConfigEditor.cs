// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Configs;
using Code.Editor.Common;

using UnityEditor;
using UnityEngine;

namespace Code.Editor.Configs
{
  [CustomEditor(typeof(GameConfig))]
  [ManualSaveInspector]
  public sealed class GameConfigEditor : ManualSaveEditor
  {
    private bool _globalFoldout = true;

    protected override void DrawInspector()
    {
      DrawGlobalSettings();
    }

    private void DrawGlobalSettings()
    {
      _globalFoldout =
        EditorGUILayout.BeginFoldoutHeaderGroup(_globalFoldout, "Global Settings");

      if (_globalFoldout)
      {
        SerializedProperty playerTag =
          serializedObject.FindProperty(nameof(GameConfig.PlayerTag));
        SerializedProperty playerStartTag =
          serializedObject.FindProperty(nameof(GameConfig.PlayerStartTag));
        SerializedProperty enemySpawnerTag =
          serializedObject.FindProperty(nameof(GameConfig.EnemySpawnerTag));

        SerializedProperty playerLayer =
          serializedObject.FindProperty(nameof(GameConfig.PlayerLayer));
        SerializedProperty hitableLayer =
          serializedObject.FindProperty(nameof(GameConfig.EnemyHitableLayer));

        playerTag.stringValue =
          EditorGUILayout.TagField("Player Tag", playerTag.stringValue);

        playerStartTag.stringValue =
          EditorGUILayout.TagField("Player Start Tag", playerStartTag.stringValue);

        enemySpawnerTag.stringValue =
          EditorGUILayout.TagField("Enemy Spawner Tag", enemySpawnerTag.stringValue);

        EditorGUILayout.PropertyField(
          serializedObject.FindProperty(nameof(GameConfig.EnemyDisappearDelay)),
          new GUIContent("Enemy Disappear Delay"));

        playerLayer.intValue =
          EditorGUILayout.LayerField("Player Layer", playerLayer.intValue);

        hitableLayer.intValue =
          EditorGUILayout.LayerField("Enemy Hitable Layer", hitableLayer.intValue);
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
    }

  }
}
