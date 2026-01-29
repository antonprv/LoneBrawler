// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Common;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(PlayerStaticData))]
  [ManualSaveInspector]
  public class PlayerStaticDataEditor : ManualSaveEditor
  {
    private bool _playerFoldout = true;

    protected override void DrawInspector()
    {
      DrawPlayerSettings();
    }

    private void DrawPlayerSettings()
    {
      _playerFoldout =
        EditorGUILayout.BeginFoldoutHeaderGroup(_playerFoldout, "Player Settings");

      if (_playerFoldout)
      {
        EditorGUILayout.PropertyField(
          serializedObject.FindProperty(nameof(PlayerStaticData.PlayerMaxHealth)),
          new GUIContent("Max Health"));

        EditorGUILayout.PropertyField(
          serializedObject.FindProperty(nameof(PlayerStaticData.PlayerAttackDamage)),
          new GUIContent("Player Attack Damage"));

        EditorGUILayout.PropertyField(
          serializedObject.FindProperty(nameof(PlayerStaticData.PlayerAttackRange)),
          new GUIContent("Player Attack Range"));

        EditorGUILayout.PropertyField(
          serializedObject.FindProperty(nameof(PlayerStaticData.PlayerAttackRadius)),
          new GUIContent("Player Attack Radius"));

        EditorGUILayout.PropertyField(
          serializedObject.FindProperty(nameof(PlayerStaticData.PlayerMaxEnemiesHit)),
          new GUIContent("Player Max Enemies Hit"));
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
    }
  }
}
