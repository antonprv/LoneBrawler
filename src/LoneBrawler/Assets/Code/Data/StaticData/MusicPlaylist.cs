// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "MusicPlaylist", menuName = "StaticData/Audio/MusicPlaylist")]
  public class MusicPlaylist : ScriptableObject
  {
    [Tooltip("Addressable references to audio clips in this playlist. " +
             "Tracks are loaded asynchronously before playback and released when no longer needed.")]
    public AssetReferenceT<AudioClip>[] tracks = System.Array.Empty<AssetReferenceT<AudioClip>>();

    [Tooltip("When enabled, tracks play in a randomised order using Fisher-Yates shuffle. " +
             "Has no effect when the playlist contains only one track. " +
             "The order is reshuffled every time the playlist loops.")]
    public bool shuffle;

    [Tooltip("When enabled, the playlist restarts automatically after the last track finishes. " +
             "When disabled, music fades out after the final track ends.")]
    public bool loop = true;
  }
}
