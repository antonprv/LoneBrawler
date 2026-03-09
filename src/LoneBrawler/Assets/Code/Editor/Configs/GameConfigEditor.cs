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
    private bool _monetizationFoldout = true;

    private const float _foldoutSpaces = 8f;

    protected override void DrawInspector()
    {
      InspectorUtils.DrawFoldout(
        serializedObject,
        "Gameplay Tags",
        ref _gameplayTagsFoldout,
        GameplayTagFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Physics Layers",
        ref _physicsLayersFoldout,
        PhysicsLayersFields
        );

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Monetization",
        ref _monetizationFoldout,
        MonetizationFields
        );
    }

    private static readonly string[] GameplayTagFields =
    {
      nameof(GameConfig.PlayerTag),
      nameof(GameConfig.PlayerStartTag),
      nameof(GameConfig.EnemyTag),
      nameof(GameConfig.EnemySpawnerTag)
    };

    private static readonly string[] PhysicsLayersFields =
{
      nameof(GameConfig.PlayerLayer),
      nameof(GameConfig.EnemyHitableLayer),
      nameof(GameConfig.LootLayer),
      nameof(GameConfig.AggroLayer),
      nameof(GameConfig.AttackZoneLayer),
      nameof(GameConfig.SaveTriggerLayer)
    };

    private static readonly string[] MonetizationFields =
    {
      nameof(GameConfig.RewardedAddSouls)
    };
  }
}
