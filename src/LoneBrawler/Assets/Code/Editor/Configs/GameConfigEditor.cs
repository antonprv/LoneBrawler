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
    }

    private void DrawGameplayTags()
    {
      _gameplayTagsFoldout =
        EditorGUILayout.BeginFoldoutHeaderGroup(_gameplayTagsFoldout, "Gameplay Tags");

      if (_gameplayTagsFoldout)
      {
        SerializedProperty playerStartTag =
          serializedObject.FindProperty(nameof(GameConfig.PlayerStartTag));

        playerStartTag.stringValue =
          EditorGUILayout.TagField("Player Start Tag", playerStartTag.stringValue);
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

        playerLayer.intValue =
          EditorGUILayout.LayerField("Player Layer", playerLayer.intValue);
        hitableLayer.intValue =
          EditorGUILayout.LayerField("Enemy Hitable Layer", hitableLayer.intValue);
        lootLayer.intValue =
          EditorGUILayout.LayerField("Loot Layer", lootLayer.intValue);
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
    }
  }
}
