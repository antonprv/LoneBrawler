// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Threading;

using Code.Data.StaticData;

using Code.Data.StaticData.Configs;
using Code.Gameplay.Audio.Music.DataExtensions;
using Code.Gameplay.Audio.Music.Interfaces;

using Code.Infrastructure.Services.Random;
using Code.Infrastructure.Services.SoundService.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Audio.Music
{
  /// <summary>
  /// Orchestrates background music: playlist sequencing, async clip loading,
  /// fade-in / fade-out, and crossfade transitions.
  ///
  /// Requires two AudioSources assigned in the Inspector, both with Loop disabled.
  /// The active source plays the current track; the staging source is prepared during
  /// a crossfade. They swap roles after every crossfade completes.
  ///
  /// Performance optimisations:
  ///   Zero-track playlist — all public methods are safe no-ops; nothing is loaded.
  ///   Single-track playlist — delegates looping to native AudioSource.loop, bypassing
  ///                           the entire crossfade and auto-advance machinery.
  ///   Multi-track playlist  — pre-loads the next track while the current one plays so
  ///                           the clip is already cached when the crossfade begins.
  ///
  /// Depends on:
  ///   <see cref="IFader"/>          — volume interpolation
  ///   <see cref="ITrackSequencer"/> — track ordering and shuffle
  ///   <see cref="ITrackLoader"/>    — async Addressables loading and caching
  ///   <see cref="ISoundService"/>   — user-controlled volume setting
  /// </summary>
  public class MusicPlayer : ZenjexBehaviour, IMusicPlayer
  {
    [Header("Audio Sources")]
    [Tooltip("Plays the current track. Loop must be disabled; MusicPlayer manages looping.")]
    public AudioSource activeSource;

    [Tooltip("Used as the crossfade target. Loop must be disabled.")]
    public AudioSource stagingSource;

    [Header("Configuration")]
    [Tooltip("Timing parameters for fades and crossfades.")]
    public MusicPlayerConfig config;

    [Tooltip("Playlist that starts playing when Play() is first called. Optional.")]
    public MusicPlaylist defaultPlaylist;

    #region Dependencies

    [Zenjex] private readonly ISoundService _soundService;
    [Zenjex] private readonly IRandomService _random;
    [Zenjex] private readonly IFader _fader;
    [Zenjex] private readonly ITrackLoader _trackLoader;

    #endregion

    #region State

    private ITrackSequencer _sequencer;
    private float _targetVolume;
    private CancellationTokenSource _sessionCts;
    private readonly CompositeDisposable _disposables = new();

    #endregion

    #region Lifecycle

    public void Construct()
    {
      _sequencer = new TrackSequencer(_random);

      ResetSourceVolumes();
      SubscribeToVolumeChanges();

      if (defaultPlaylist != null)
        SetPlaylist(defaultPlaylist);
    }

    private void OnDestroy()
    {
      EndSession();
      _trackLoader.ReleaseAll();
      _disposables.Dispose();
    }

    #endregion

    #region IMusicPlayer

    public void SetPlaylist(MusicPlaylist playlist)
    {
      _trackLoader.ReleaseAll();
      _sequencer.Load(playlist);
    }

    /// <summary>
    /// Loads the first track, then fades volume in from silence.
    /// Single-track playlists use native AudioSource looping — no crossfade overhead.
    /// </summary>
    public async UniTask Play()
    {
      if (!_sequencer.IsLoaded)
      {
        Debug.LogWarning("[MusicPlayer] Cannot Play: no playlist loaded or playlist is empty.");
        return;
      }

      var ct = StartNewSession();
      _targetVolume = _soundService.MusicVolumeRP.CurrentValue;

      var clip = await _trackLoader.LoadAsync(_sequencer.Current, ct);
      if (ct.IsCancellationRequested) return;

      PrepareSource(activeSource, clip, volume: 0f);

      if (_sequencer.IsSingleTrack)
      {
        // Let the engine handle looping natively — zero per-frame overhead.
        activeSource.loop = _sequencer.IsLooping;
        activeSource.Play();
        await _fader.Fade(activeSource, from: 0f, to: _targetVolume, config.fadeInDuration, ct);

        // No AutoAdvanceLoop needed: either the engine loops, or the track plays once and stops.
        return;
      }

      activeSource.loop = false;
      activeSource.Play();

      PreloadNext();

      await _fader.Fade(activeSource, from: 0f, to: _targetVolume, config.fadeInDuration, ct);

      if (!ct.IsCancellationRequested)
        AutoAdvanceLoop(ct).Forget();
    }

    /// <summary>
    /// Fades the active source to silence and stops it.
    /// Resets native loop flag so a subsequent Play() starts cleanly.
    /// </summary>
    public async UniTask Stop()
    {
      var ct = StartNewSession();

      activeSource.loop = false;

      await _fader.Fade(activeSource, activeSource.volume, to: 0f, config.fadeOutDuration, ct);

      if (!ct.IsCancellationRequested)
        activeSource.Stop();
    }

    /// <summary>
    /// Crossfades to the first track of a new playlist.
    /// Releases all previously loaded clips and pre-loads the next track after the crossfade.
    /// </summary>
    public async UniTask CrossfadeTo(MusicPlaylist playlist)
    {
      if (playlist == null || !playlist.IsValid())
      {
        Debug.LogWarning("[MusicPlayer] CrossfadeTo received a null or empty playlist.");
        return;
      }

      _trackLoader.ReleaseAll();
      _sequencer.Load(playlist);

      var ct = StartNewSession();

      activeSource.loop = false;

      var clip = await _trackLoader.LoadAsync(_sequencer.Current, ct);
      if (ct.IsCancellationRequested) return;

      await ExecuteCrossfade(clip, ct);

      if (ct.IsCancellationRequested) return;

      if (_sequencer.IsSingleTrack)
      {
        activeSource.loop = _sequencer.IsLooping;
        return;
      }

      PreloadNext();
      AutoAdvanceLoop(ct).Forget();
    }

    /// <summary>
    /// Crossfades to the next track in the current playlist.
    /// Fades out with Stop() if the playlist is exhausted.
    /// </summary>
    public async UniTask SkipToNext()
    {
      var nextRef = _sequencer.Advance();

      if (nextRef == null)
      {
        await Stop();
        return;
      }

      var ct = StartNewSession();

      activeSource.loop = false;

      var clip = await _trackLoader.LoadAsync(nextRef, ct);
      if (ct.IsCancellationRequested) return;

      await ExecuteCrossfade(clip, ct);

      if (ct.IsCancellationRequested) return;

      PreloadNext();
      AutoAdvanceLoop(ct).Forget();
    }

    #endregion

    #region Auto-advance

    /// <summary>
    /// Runs for the lifetime of a multi-track playback session.
    /// Waits until just before each track ends, then crossfades to the next.
    /// The next clip is always pre-loaded during the wait, so the crossfade
    /// begins without an async stall.
    /// </summary>
    private async UniTaskVoid AutoAdvanceLoop(CancellationToken ct)
    {
      while (!ct.IsCancellationRequested)
      {
        float waitSeconds = SecondsUntilCrossfadeStart();

        if (waitSeconds > 0f)
          await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: ct);

        if (ct.IsCancellationRequested)
          return;

        var nextRef = _sequencer.Advance();

        if (nextRef == null)
        {
          // Non-looping playlist exhausted — fade out and stop.
          await _fader.Fade(activeSource, activeSource.volume, to: 0f, config.crossfadeDuration, ct);
          if (!ct.IsCancellationRequested)
            activeSource.Stop();
          return;
        }

        // Clip should already be cached from PreloadNext(); LoadAsync returns immediately.
        var clip = await _trackLoader.LoadAsync(nextRef, ct);
        if (ct.IsCancellationRequested) return;

        await ExecuteCrossfade(clip, ct);

        if (ct.IsCancellationRequested) return;

        // Release the previous track now that the crossfade is complete.
        _trackLoader.ReleaseExcept(_sequencer.Current);

        // Pre-load the track after next so it's ready for the following crossfade.
        PreloadNext();
      }
    }

    /// <summary>
    /// Seconds remaining in the active clip minus the crossfade window.
    /// Returns zero when no clip is assigned or the clip is shorter than the crossfade.
    /// </summary>
    private float SecondsUntilCrossfadeStart()
    {
      if (activeSource.clip == null)
        return 0f;

      float remaining = activeSource.clip.length - activeSource.time;
      return Mathf.Max(0f, remaining - config.crossfadeDuration);
    }

    #endregion

    #region Crossfade

    /// <summary>
    /// Simultaneously fades out the active source and fades in the staging source.
    /// Stops the outgoing source and swaps source roles on completion.
    /// </summary>
    private async UniTask ExecuteCrossfade(AudioClip nextClip, CancellationToken ct)
    {
      PrepareSource(stagingSource, nextClip, volume: 0f);
      stagingSource.Play();

      float volumeAtCrossfadeStart = activeSource.volume;

      await UniTask.WhenAll(
        _fader.Fade(activeSource, volumeAtCrossfadeStart, to: 0f, config.crossfadeDuration, ct),
        _fader.Fade(stagingSource, from: 0f, to: _targetVolume, config.crossfadeDuration, ct)
      );

      if (ct.IsCancellationRequested)
        return;

      activeSource.Stop();
      SwapSources();
    }

    #endregion

    #region Pre-loading

    /// <summary>
    /// Fires a background load for the next track so it is cached before the crossfade.
    /// PeekNext returns null for single-track or exhausted non-looping playlists — both
    /// are handled inside <see cref="ITrackLoader.Preload"/>.
    /// </summary>
    private void PreloadNext() =>
      _trackLoader.Preload(_sequencer.PeekNext);

    #endregion

    #region Volume sync

    private void SubscribeToVolumeChanges()
    {
      _soundService.MusicVolumeRP
        .Skip(1)
        .Subscribe(ApplyNewTargetVolume)
        .AddTo(_disposables);
    }

    /// <summary>
    /// Updates the cached target volume.
    /// Applies it directly to the active source only when no fade session is running.
    /// </summary>
    private void ApplyNewTargetVolume(float newVolume)
    {
      _targetVolume = newVolume;

      if (activeSource.isPlaying && _sessionCts == null)
        activeSource.volume = newVolume;
    }

    #endregion

    #region Session Management

    /// <summary>
    /// Cancels the in-progress session and creates a fresh cancellation token.
    /// Every public async method calls this to ensure only one operation runs at a time.
    /// </summary>
    private CancellationToken StartNewSession()
    {
      EndSession();
      _sessionCts = new CancellationTokenSource();
      return _sessionCts.Token;
    }

    private void EndSession()
    {
      if (_sessionCts == null) return;
      _sessionCts.Cancel();
      _sessionCts.Dispose();
      _sessionCts = null;
    }

    #endregion

    #region Utilities

    private static void PrepareSource(AudioSource source, AudioClip clip, float volume)
    {
      source.clip = clip;
      source.time = 0f;
      source.volume = volume;
      source.loop = false;
    }

    private void SwapSources() =>
      (activeSource, stagingSource) = (stagingSource, activeSource);

    private void ResetSourceVolumes()
    {
      activeSource.volume = 0f;
      stagingSource.volume = 0f;
    }

    #endregion
  }
}
