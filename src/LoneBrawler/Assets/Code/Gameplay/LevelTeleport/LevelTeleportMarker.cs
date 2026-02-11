// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.LevelTeleport
{
  public class LevelTeleportMarker : MonoBehaviour
  {
    public string UniqueName;

    public string LevelKey;

    public TeleportEnterMarker EnterMarker;

    [Tooltip("Enable if teleport is within the same level (disables cross-level validation)")]
    public bool TeleportsToSameLevel;
  }
}
