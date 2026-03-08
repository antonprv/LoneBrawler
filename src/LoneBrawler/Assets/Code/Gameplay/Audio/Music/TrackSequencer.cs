// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

using Code.Gameplay.Audio.Music.Interfaces;

using Code.Infrastructure.Services.Random;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Gameplay.Audio.Music
{
  /// <summary>
  /// Determines the playback order of Addressable track references in a MusicPlaylist.
  ///
  /// Optimisations:
  ///   Zero tracks  — <see cref="IsLoaded"/> returns false; all other members are safe no-ops.
  ///   Single track  — shuffle is skipped entirely; <see cref="IsSingleTrack"/> lets
  ///                   MusicPlayer delegate looping to the native AudioSource.
  ///   Multiple tracks — Fisher-Yates shuffle applied on load and on every loop restart.
  /// </summary>
  public class TrackSequencer : ITrackSequencer
  {
    private readonly IRandomService _random;

    private MusicPlaylist _playlist;
    private int[] _order;
    private int _index;

    #region Class Fields

    public AssetReferenceT<AudioClip> Current =>
      IsLoaded ? _playlist.tracks[_order[_index]] : null;

    public AssetReferenceT<AudioClip> PeekNext
    {
      get
      {
        if (!IsLoaded || IsSingleTrack)
          return null;

        int nextIndex = _index + 1;

        if (nextIndex >= _order.Length)
          return _playlist.loop ? _playlist.tracks[_order[0]] : null;

        return _playlist.tracks[_order[nextIndex]];
      }
    }

    public bool IsLoaded => _playlist != null && _playlist.tracks.Length > 0;
    public bool IsSingleTrack => _playlist != null && _playlist.tracks.Length == 1;
    public bool IsLooping => _playlist != null && _playlist.loop;

    #endregion

    public TrackSequencer(IRandomService random) =>
      _random = random;

    /// <inheritdoc/>
    public void Load(MusicPlaylist playlist)
    {
      _playlist = playlist;
      _index = 0;
      BuildOrder();
    }

    /// <inheritdoc/>
    public AssetReferenceT<AudioClip> Advance()
    {
      if (!IsLoaded)
        return null;

      // Single-track playlists are looped natively by AudioSource; Advance() should not be called.
      // Guard here in case it is called anyway.
      if (IsSingleTrack)
        return _playlist.loop ? Current : null;

      _index++;

      if (_index >= _order.Length)
      {
        if (!_playlist.loop)
          return null; // signal: playlist exhausted, begin fade-out

        _index = 0;

        if (_playlist.shuffle)
          Shuffle(); // reshuffle so each loop sounds different
      }

      return Current;
    }

    #region Private Methods

    private void BuildOrder()
    {
      int count = _playlist.tracks.Length;
      _order = new int[count];

      for (int i = 0; i < count; i++)
        _order[i] = i;

      // Shuffling a single-element array is a no-op, but we skip it explicitly
      // to avoid allocating the random call and communicating intent clearly.
      if (_playlist.shuffle && !IsSingleTrack)
        Shuffle();
    }

    /// <summary>Fisher-Yates in-place shuffle.</summary>
    private void Shuffle()
    {
      for (int i = _order.Length - 1; i > 0; i--)
      {
        int j = _random.Range(0, i + 1, true);
        (_order[i], _order[j]) = (_order[j], _order[i]);
      }
    }

    #endregion

  }
}
