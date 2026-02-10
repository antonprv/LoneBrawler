// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs;
using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.Configs
{
  [CustomEditor(typeof(GameConfig))]
  public sealed class GameConfigEditor : ManualSaveEditor
  {
    private bool _gameplayTagsFoldout = true;
    private bool _physicsLayersFoldout = true;

    private const float _foldoutSpaces = 8f;

    protected override void DrawInspector()
    {
      DrawGameplayTags();
      EditorGUILayout.Space(_foldoutSpaces);
      DrawPhysicsLayers();
      EditorGUILayout.Space(_foldoutSpaces);
    }

    private void DrawGameplayTags()
    {
      _gameplayTagsFoldout =
        EditorGUILayout.BeginFoldoutHeaderGroup(_gameplayTagsFoldout, "Gameplay Tags");

      if (_gameplayTagsFoldout)
      {
        SerializedProperty playerTag =
          serializedObject.FindProperty(nameof(GameConfig.PlayerTag));
        SerializedProperty playerStartTag =
          serializedObject.FindProperty(nameof(GameConfig.PlayerStartTag));
        SerializedProperty enemyTag =
          serializedObject.FindProperty(nameof(GameConfig.EnemyTag));
        SerializedProperty enemySpawnerTag =
          serializedObject.FindProperty(nameof(GameConfig.EnemySpawnerTag));

        playerTag.stringValue =
          EditorGUILayout.TagField("Player Tag", playerTag.stringValue);
        playerStartTag.stringValue =
          EditorGUILayout.TagField("Player Start Tag", playerStartTag.stringValue);
        enemyTag.stringValue =
          EditorGUILayout.TagField("Enemy Tag", enemyTag.stringValue);
        enemySpawnerTag.stringValue =
          EditorGUILayout.TagField("Enemy Spawner Tag", enemySpawnerTag.stringValue);
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawPhysicsLayers()
    {
      _physicsLayersFoldout =
        EditorGUILayout.BeginFoldoutHeaderGroup(_physicsLayersFoldout, "Global Settings");

      if (_physicsLayersFoldout)
      {
        SerializedProperty playerLayer =
          serializedObject.FindProperty(nameof(GameConfig.PlayerLayer));
        SerializedProperty hitableLayer =
          serializedObject.FindProperty(nameof(GameConfig.EnemyHitableLayer));
        SerializedProperty lootLayer =
          serializedObject.FindProperty(nameof(GameConfig.LootLayer));
        SerializedProperty aggroLayer =
          serializedObject.FindProperty(nameof(GameConfig.AggroLayer));
        SerializedProperty attackZoneLayer =
          serializedObject.FindProperty(nameof(GameConfig.AttackZoneLayer));
        SerializedProperty saveTriggerLayer =
          serializedObject.FindProperty(nameof(GameConfig.SaveTriggerLayer));

        playerLayer.intValue =
          EditorGUILayout.LayerField("Player Layer", playerLayer.intValue);
        hitableLayer.intValue =
          EditorGUILayout.LayerField("Enemy Hitable Layer", hitableLayer.intValue);
        lootLayer.intValue =
          EditorGUILayout.LayerField("Loot Layer", lootLayer.intValue);
        aggroLayer.intValue =
          EditorGUILayout.LayerField("Aggro Layer", aggroLayer.intValue);
        attackZoneLayer.intValue =
          EditorGUILayout.LayerField("Attack Zone Layer", attackZoneLayer.intValue);
        saveTriggerLayer.intValue =
          EditorGUILayout.LayerField("Save Trigger Layer", saveTriggerLayer.intValue);
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
    }
  }
}
