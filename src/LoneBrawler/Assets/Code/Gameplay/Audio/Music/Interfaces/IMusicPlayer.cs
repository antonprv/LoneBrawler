// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Configs;

using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Audio.Music.Interfaces
{
  /// <summary>
  /// Controls background music playback with fade and crossfade transitions.
  /// Consumers depend on this interface; they never reference MusicPlayer directly.
  /// </summary>
  public interface IMusicPlayer
  {
    /// <summary>
    /// Assigns a playlist without starting playback.
    /// Useful for pre-loading the correct playlist before calling <see cref="Play"/>.
    /// </summary>
    void SetPlaylist(MusicPlaylist playlist);

    /// <summary>
    /// Starts playback of the current playlist.
    /// Fades volume from zero up to the value in <c>ISoundService.MusicVolumeRP</c>.
    /// </summary>
    UniTask Play();

    /// <summary>
    /// Fades volume to zero and stops the active audio source.
    /// </summary>
    UniTask Stop();

    /// <summary>
    /// Performs a crossfade from the current track to the first track of <paramref name="playlist"/>.
    /// Safe to call while already playing; the old session is cancelled cleanly.
    /// </summary>
    UniTask CrossfadeTo(MusicPlaylist playlist);

    /// <summary>
    /// Crossfades to the next track in the current playlist.
    /// Intended for a "skip" button or manual scene transitions.
    /// </summary>
    UniTask SkipToNext();
    void SetConfig(MusicPlayerConfig playerConfig);
  }
}
