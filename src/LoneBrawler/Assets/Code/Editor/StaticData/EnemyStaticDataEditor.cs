// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(EnemyStaticData))]
  public class EnemyStaticDataEditor : ManualSaveEditor
  {
    private bool _typeIdData = true;
    private bool _attackData = true;
    private bool _healthData = true;
    private bool _deathData = true;
    private bool _moveData = true;
    private bool _soulsData = true;
    private bool _prefabData = true;

    private const int FoldoutSpaces = 10;

    protected override void DrawInspector()
    {
      InspectorUtils.DrawFoldout(
        serializedObject,
        "Type Parameters",
        ref _typeIdData,
        TypeIdFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Attack",
        ref _attackData,
        AttackFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Health Parameters",
        ref _healthData,
        HealthFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Death Parameters",
        ref _deathData,
        DeathFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Move Parameters",
        ref _moveData,
        MoveFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Souls Parameters",
        ref _soulsData,
        SoulsFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Prefabs",
        ref _prefabData,
        PrefabFields);
    }

    private static readonly string[] TypeIdFields =
    {
      nameof(EnemyStaticData.EnemyTypeId),
      nameof(EnemyStaticData.IsContainer),
      nameof(EnemyStaticData.ShouldMove)
    };

    // EnemyAttackType + preset reference + enemy-specific behavior parameters
    private static readonly string[] AttackFields =
    {
      nameof(EnemyStaticData.EnemyAttackType),
      nameof(EnemyStaticData.AttackPresetReference),
      nameof(EnemyStaticData.AttackCooldown),
      nameof(EnemyStaticData.HitRecoverCooldown),
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
      nameof(EnemyStaticData.PrefabReference),
      nameof(EnemyStaticData.LootPrefabReference)
    };
  }
}
