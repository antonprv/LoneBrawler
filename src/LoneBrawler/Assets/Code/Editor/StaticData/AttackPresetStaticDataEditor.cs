// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(AttackPresetStaticData))]
  public class AttackPresetStaticDataEditor : ManualSaveEditor
  {
    private bool _identityData = true;
    private bool _timingData = true;
    private bool _damageData = true;
    private bool _projectileData = true;
    private bool _visualData = true;

    private const int FoldoutSpaces = 10;

    protected override void DrawInspector()
    {
      InspectorUtils.DrawFoldout(
        serializedObject,
        "Identity",
        ref _identityData,
        IdentityFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Timing",
        ref _timingData,
        TimingFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Damage & Range",
        ref _damageData,
        DamageFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Ranged Projectile",
        ref _projectileData,
        ProjectileFields);

      EditorGUILayout.Space(FoldoutSpaces);

      InspectorUtils.DrawFoldout(
        serializedObject,
        "Visuals",
        ref _visualData,
        VisualFields);
    }

    private static readonly string[] IdentityFields =
    {
      nameof(AttackPresetStaticData.PresetId),
      nameof(AttackPresetStaticData.TargetMode)
    };

    private static readonly string[] TimingFields =
    {
      nameof(AttackPresetStaticData.WindupDuration),
      nameof(AttackPresetStaticData.HitWindowDuration),
      nameof(AttackPresetStaticData.RecoveryDuration)
    };

    private static readonly string[] DamageFields =
    {
      nameof(AttackPresetStaticData.Damage),
      nameof(AttackPresetStaticData.AttackStartRange),
      nameof(AttackPresetStaticData.AreaRadius)
    };

    private static readonly string[] ProjectileFields =
    {
      nameof(AttackPresetStaticData.ProjectilePrefab),
      nameof(AttackPresetStaticData.ProjectileSpeed)
    };

    private static readonly string[] VisualFields =
    {
      nameof(AttackPresetStaticData.CastVfxPrefab),
      nameof(AttackPresetStaticData.HitVfxPrefab)
    };
  }
}
