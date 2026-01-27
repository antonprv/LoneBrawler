// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Configs;

using UnityEditor;

using UnityEngine;

namespace Code.Configs
{
  [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
  public class GameConfig : ScriptableObject
  {
    public string PlayerTag;
    public string PlayerStartTag;

    public int PlayerLayer;
  }

  public static class GameConfiguration
  {
    public static string PlayerTag => GetConfiguration().PlayerTag;
    public static string PlayerStartTag => GetConfiguration().PlayerStartTag;
    public static int PlayerCollision => 1 << GetConfiguration().PlayerLayer;

    private static GameConfig _gameconfig;

    private static GameConfig GetConfiguration()
    {
      if (!_gameconfig)
      {
        _gameconfig = Resources.Load<GameConfig>("Config/GameConfig");

        if (!_gameconfig)
        {
          Debug.LogError("BuildConfig not found! Make sure it's in a Resources folder with correct path");
          _gameconfig = ScriptableObject.CreateInstance<GameConfig>();
        }
      }
      return _gameconfig;
    }
  }
}

[CustomEditor(typeof(GameConfig))]
public sealed class GameConfigEditor : Editor
{
  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    SerializedProperty playerTag = serializedObject.FindProperty(nameof(GameConfig.PlayerTag));
    SerializedProperty playerStartTag = serializedObject.FindProperty(nameof(GameConfig.PlayerStartTag));
    SerializedProperty playerLayer = serializedObject.FindProperty(nameof(GameConfig.PlayerLayer));

    playerTag.stringValue =
      EditorGUILayout.TagField("Player Tag", playerTag.stringValue);

    playerStartTag.stringValue =
      EditorGUILayout.TagField("Player Start Tag", playerStartTag.stringValue);

    playerLayer.intValue =
      EditorGUILayout.LayerField("Player Layer", playerLayer.intValue);

    serializedObject.ApplyModifiedProperties();
  }
}
