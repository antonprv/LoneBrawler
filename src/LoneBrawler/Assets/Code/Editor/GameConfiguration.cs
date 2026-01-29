// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(Code.Configs.GameConfig))]
public sealed class GameConfigEditor : Editor
{
  private bool _globalFoldout = true;
  private bool _playerFoldout = true;

  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    DrawGlobalSettings();
    DrawPlayerSettings();

    serializedObject.ApplyModifiedProperties();
  }

  private void DrawGlobalSettings()
  {
    _globalFoldout =
      EditorGUILayout.BeginFoldoutHeaderGroup(_globalFoldout, "Global Settings");

    if (_globalFoldout)
    {
      SerializedProperty playerTag =
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerTag));
      SerializedProperty playerStartTag =
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerStartTag));
      SerializedProperty playerLayer =
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerLayer));
      SerializedProperty hitableLayer =
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.EnemyHitableLayer));

      playerTag.stringValue =
        EditorGUILayout.TagField("Player Tag", playerTag.stringValue);

      playerStartTag.stringValue =
        EditorGUILayout.TagField("Player Start Tag", playerStartTag.stringValue);

      EditorGUILayout.PropertyField(
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.EnemyDisappearDelay)),
        new GUIContent("Enemy Disappear Delay"));

      playerLayer.intValue =
        EditorGUILayout.LayerField("Player Layer", playerLayer.intValue);

      hitableLayer.intValue =
        EditorGUILayout.LayerField("Enemy Hitable Layer", hitableLayer.intValue);
    }

    EditorGUILayout.EndFoldoutHeaderGroup();
  }

  private void DrawPlayerSettings()
  {
    _playerFoldout =
      EditorGUILayout.BeginFoldoutHeaderGroup(_playerFoldout, "Player Settings");

    if (_playerFoldout)
    {
      EditorGUILayout.PropertyField(
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerMaxHealth)),
        new GUIContent("Max Health"));

      EditorGUILayout.PropertyField(
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerAttackDamage)),
        new GUIContent("Player Attack Damage"));

      EditorGUILayout.PropertyField(
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerAttackRange)),
        new GUIContent("Player Attack Range"));

      EditorGUILayout.PropertyField(
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerAttackRadius)),
        new GUIContent("Player Attack Radius"));

      EditorGUILayout.PropertyField(
        serializedObject.FindProperty(nameof(Code.Configs.GameConfig.PlayerMaxEnemiesHit)),
        new GUIContent("Player Max Enemies Hit"));
    }

    EditorGUILayout.EndFoldoutHeaderGroup();
  }
}
