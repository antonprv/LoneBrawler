// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Spawn;
using Code.Gameplay.LevelTeleport;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.CustomGismos
{
  [CustomEditor(typeof(LevelTeleportMarker))]
  class TeleportTriggerEditor : UnityEditor.Editor
  {
    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void DrawTeleportTriggerGizmo(LevelTeleportMarker teleport, GizmoType gizmoType)
    {
      Color gizmoColor =
        ColorUtility.TryParseHtmlString("#7ebd18", out var c) ? c : Color.white;

      Gizmos.color = gizmoColor;
      Gizmos.DrawCube(teleport.transform.position, teleport.transform.localScale);
    }
  }
}
