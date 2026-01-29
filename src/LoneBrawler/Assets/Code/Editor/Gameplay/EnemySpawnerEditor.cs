// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Spawn;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Gameplay
{
  [CustomEditor(typeof(EnemySpawner))]
  public class EnemySpawnerEditor : UnityEditor.Editor
  {
    private static readonly float _gizmoSize = 0.8f;

    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void DrawSpawnerGizmo(EnemySpawner spawner, GizmoType gizmoType)
    {
      Color gizmoColor =
        ColorUtility.TryParseHtmlString("#F91D62", out var c) ? c : Color.white;

      Gizmos.color = gizmoColor;
      Gizmos.DrawSphere(spawner.transform.position, _gizmoSize);
    }
  }
}
