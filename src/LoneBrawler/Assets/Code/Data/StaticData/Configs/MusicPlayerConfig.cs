// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData.Configs
{
  /// <summary>
  /// Timing parameters for all fade and crossfade operations in MusicPlayer.
  /// Create via Assets → Sound → Music Player Config.
  /// </summary>
  [CreateAssetMenu(fileName = "MusicPlayerConfig", menuName = "StaticData/Config/MusicPlayerConfig")]
  public class MusicPlayerConfig : ScriptableObject
  {
    [Tooltip("How long, in seconds, volume rises from silence to target when Play() is called.")]
    [Range(0.1f, 10f)]
    public float fadeInDuration = 1.5f;

    [Tooltip("How long, in seconds, volume falls to silence when Stop() is called.")]
    [Range(0.1f, 10f)]
    public float fadeOutDuration = 1.5f;

    [Tooltip("How long, in seconds, the outgoing and incoming tracks overlap during a crossfade. " +
             "The auto-advance system starts a crossfade this many seconds before the current track ends.")]
    [Range(0.1f, 10f)]
    public float crossfadeDuration = 2f;
  }
}
