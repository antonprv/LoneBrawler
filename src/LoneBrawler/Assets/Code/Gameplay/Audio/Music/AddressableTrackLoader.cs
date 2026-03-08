// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Threading;

using Code.Gameplay.Audio.Music.Interfaces;

using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Gameplay.Audio.Music
{
  /// <summary>
  /// Loads AudioClip assets via Addressables and maintains a per-session cache.
  ///
  /// Memory strategy:
  ///   At most two clips are resident at the same time: the currently playing track
  ///   and the next one being pre-loaded. After every crossfade, the previous track
  ///   is released via <see cref="ReleaseExcept"/>.
  ///
  ///   Clip handles are managed independently from the shared <see cref="IAssetLoader"/> cache
  ///   so that releasing a music clip never affects other systems.
  /// </summary>
  public class AddressableTrackLoader : ITrackLoader
  {
    private readonly IAssetLoader _assetLoader;

    /// <summary>GUID → loaded AudioClip. Never holds more than two entries during normal playback.</summary>
    private readonly Dictionary<string, AudioClip> _cache = new();

    public AddressableTrackLoader(IAssetLoader assetLoader) =>
      _assetLoader = assetLoader;

    /// <inheritdoc/>
    public async UniTask<AudioClip> LoadAsync(AssetReferenceT<AudioClip> reference, CancellationToken ct)
    {
      string guid = reference.AssetGUID;

      if (_cache.TryGetValue(guid, out AudioClip cached))
        return cached;

      AudioClip clip = await _assetLoader.LoadAsync<AudioClip>(reference);

      if (ct.IsCancellationRequested)
        return null;

      // Guard against a concurrent load that already populated the cache
      // while this request was in flight (e.g. Preload + LoadAsync racing).
      if (!_cache.ContainsKey(guid))
        _cache[guid] = clip;

      return _cache[guid];
    }

    /// <inheritdoc/>
    public void Preload(AssetReferenceT<AudioClip> reference)
    {
      if (reference == null)
        return;

      string guid = reference.AssetGUID;

      if (_cache.ContainsKey(guid))
        return; // already loaded or loading — nothing to do

      LoadAsync(reference, CancellationToken.None).Forget();
    }

    /// <inheritdoc/>
    public void ReleaseExcept(AssetReferenceT<AudioClip> keepReference)
    {
      string keepGuid = keepReference?.AssetGUID;
      var toRemove = new List<string>(_cache.Count);

      foreach (string guid in _cache.Keys)
      {
        if (guid != keepGuid)
          toRemove.Add(guid);
      }

      foreach (string guid in toRemove)
        RemoveFromCache(guid);
    }

    /// <inheritdoc/>
    public void ReleaseAll()
    {
      var keys = new List<string>(_cache.Keys);

      foreach (string guid in keys)
        RemoveFromCache(guid);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void RemoveFromCache(string guid)
    {
      if (_cache.Remove(guid))
        Debug.Log($"[TrackLoader] Released clip {guid}");
    }
  }
}
