// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Spawn;
using Code.Gameplay.LevelTeleport;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.Markers
{
  [CustomEditor(typeof(TeleportEnterMarker))]
  public class TeleportEnterMarkerEditor : UnityEditor.Editor
  {
    private static readonly float _gizmoSize = 0.6f;

    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void DrawTeleportEnterMarkerGizmo(TeleportEnterMarker marker, GizmoType gizmoType)
    {
      Color gizmoColor = Color.coral;

      Gizmos.color = gizmoColor;
      Gizmos.DrawSphere(marker.transform.position, _gizmoSize);
      Gizmos.DrawWireSphere(marker.transform.position, _gizmoSize);
    }
  }
}
