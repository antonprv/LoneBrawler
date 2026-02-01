// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(EnemyStaticData))]
  public class EnemyStaticDataEditor : ManualSaveEditor
  {
    private bool _typeIdData = true;
    private bool _attackerData = true;
    private bool _healthData = true;
    private bool _deathData = true;
    private bool _moveData = true;
    private bool _soulsData = true;
    private bool _prefabData = true;

    private const int _foldoutSpaces = 10;

    protected override void DrawInspector()
    {
      InspectorUtils.DrawFoldout(
        serializedObject,
        "Type Id",
        ref _typeIdData,
        TypeIdFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Attack Parameters",
        ref _attackerData,
        AttackerFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Health Parameters",
        ref _healthData,
        HealthFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Death Parameters",
        ref _deathData,
        DeathFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Move Parameters",
        ref _moveData,
        MoveFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Souls Parameters",
        ref _soulsData,
        SoulsFields);

      EditorGUILayout.Space(_foldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Prefabs",
        ref _prefabData,
        PrefabFields);
    }

    private static readonly string[] TypeIdFields =
    {
      nameof(EnemyStaticData.EnemyTypeId)
    };

    private static readonly string[] AttackerFields =
    {
      nameof(EnemyStaticData.AttackRadius),
      nameof(EnemyStaticData.AttackRange),
      nameof(EnemyStaticData.AttackDamage),
      nameof(EnemyStaticData.AttackMaxHit),
      nameof(EnemyStaticData.AttackCooldown),
      nameof(EnemyStaticData.AttackTurnSpeed)
    };

    private static readonly string[] HealthFields =
    {
      nameof(EnemyStaticData.MaxHealth)
    };

    private static readonly string[] DeathFields =
    {
      nameof(EnemyStaticData.DisappearDelay)
    };

    private static readonly string[] MoveFields =
    {
      nameof(EnemyStaticData.ReachDistance),
      nameof(EnemyStaticData.AngularSpeed),
      nameof(EnemyStaticData.Speed)
    };

    private static readonly string[] SoulsFields =
    {
      nameof(EnemyStaticData.SoulsMin),
      nameof(EnemyStaticData.SoulsMax)
    };

    private static readonly string[] PrefabFields =
    {
      nameof(EnemyStaticData.Prefab),
      nameof(EnemyStaticData.LootPrefab)
    };
  }
}
