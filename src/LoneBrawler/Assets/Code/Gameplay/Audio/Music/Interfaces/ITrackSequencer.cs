// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Gameplay.Audio.Music.Interfaces
{
  /// <summary>
  /// Manages the playback order of Addressable track references within a playlist.
  /// Responsible only for sequencing logic: linear order or shuffle.
  /// Does not load assets; use <see cref="ITrackLoader"/> for that.
  /// </summary>
  public interface ITrackSequencer
  {
    /// <summary>The Addressable reference that should be playing right now.</summary>
    AssetReferenceT<AudioClip> Current { get; }

    /// <summary>
    /// The Addressable reference that will play next, without advancing the position.
    /// Returns <c>null</c> when the current track is the last one and looping is disabled.
    /// Used to trigger pre-loading while the current track is still playing.
    /// </summary>
    AssetReferenceT<AudioClip> PeekNext { get; }

    /// <summary>True when the playlist has been loaded and contains at least one track.</summary>
    bool IsLoaded { get; }

    /// <summary>
    /// True when the loaded playlist contains exactly one track.
    /// MusicPlayer can bypass crossfade logic and use native AudioSource looping instead.
    /// </summary>
    bool IsSingleTrack { get; }

    /// <summary>
    /// True when the loaded playlist loops after the last track.
    /// </summary>
    bool IsLooping { get; }

    /// <summary>
    /// Initialises the sequencer with the given playlist.
    /// Resets position to the first track and applies shuffle if needed.
    /// </summary>
    void Load(MusicPlaylist playlist);

    /// <summary>
    /// Moves to the next track and returns its Addressable reference.
    /// Returns <c>null</c> when the playlist is exhausted and looping is disabled.
    /// </summary>
    AssetReferenceT<AudioClip> Advance();
  }
}
