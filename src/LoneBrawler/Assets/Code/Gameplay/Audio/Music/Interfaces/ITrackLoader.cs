// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

// Register in GameInstaller: builder.Bind<ITrackLoader>().To<AddressableTrackLoader>().AsSingle();

using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Gameplay.Audio.Music.Interfaces
{
  /// <summary>
  /// Loads, caches, and releases Addressable AudioClip assets for music playback.
  ///
  /// Separation of concerns:
  ///   <see cref="ITrackSequencer"/> decides which track plays next.
  ///   <see cref="ITrackLoader"/>   handles when and how that track enters memory.
  ///
  /// Usage pattern in MusicPlayer:
  ///   1. Call <see cref="Preload"/> as soon as a track starts playing so the next one
  ///      loads in the background during playback.
  ///   2. Call <see cref="LoadAsync"/> just before a crossfade; the result is typically
  ///      already cached from step 1.
  ///   3. Call <see cref="ReleaseExcept"/> after a crossfade to free the previous track.
  /// </summary>
  public interface ITrackLoader
  {
    /// <summary>
    /// Returns the AudioClip for the given reference.
    /// Serves from an internal cache if already loaded; otherwise awaits the Addressables load.
    /// </summary>
    UniTask<AudioClip> LoadAsync(AssetReferenceT<AudioClip> reference, CancellationToken ct);

    /// <summary>
    /// Begins loading the clip in the background without blocking the caller.
    /// Call this as soon as the current track starts so the next one is ready when needed.
    /// Safe to call with a <c>null</c> reference (no-op).
    /// </summary>
    void Preload(AssetReferenceT<AudioClip> reference);

    /// <summary>
    /// Releases all cached clips except the one identified by <paramref name="keepReference"/>.
    /// Call this after a crossfade completes to free the memory used by the previous track.
    /// </summary>
    void ReleaseExcept(AssetReferenceT<AudioClip> keepReference);

    /// <summary>Releases all cached clips unconditionally. Call on playlist switch or scene unload.</summary>
    void ReleaseAll();
  }
}
