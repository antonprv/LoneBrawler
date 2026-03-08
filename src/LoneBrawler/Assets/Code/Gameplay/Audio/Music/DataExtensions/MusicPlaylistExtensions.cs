// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

namespace Code.Gameplay.Audio.Music.DataExtensions
{
  public static class MusicPlaylistExtensions
  {
    /// <summary>True when the playlist has at least one track assigned.</summary>
    public static bool IsValid(this MusicPlaylist playlist) =>
      playlist != null && playlist.tracks.Length > 0;

    /// <summary>
    /// True when the playlist contains exactly one track.
    /// MusicPlayer uses native AudioSource looping in this case, skipping crossfade overhead.
    /// </summary>
    public static bool IsSingleTrack(this MusicPlaylist playlist) =>
        playlist != null && playlist.tracks.Length == 1;
  }
}
