// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

// Register in GameInstaller: builder.Bind<IFader>().To<AudioFader>().AsSingle();

using System.Threading;

using Code.Gameplay.Audio.Music.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Audio.Music
{
  /// <summary>
  /// Smoothly transitions an AudioSource volume using smoothstep interpolation.
  /// Stateless: safe to share as a singleton across all music and sound components.
  /// </summary>
  public class AudioFader : IFader
  {
    /// <inheritdoc/>
    public async UniTask Fade(
      AudioSource source,
      float from,
      float to,
      float duration,
      CancellationToken ct)
    {
      if (duration <= 0f)
      {
        source.volume = to;
        return;
      }

      float elapsed = 0f;

      while (elapsed < duration)
      {
        if (ct.IsCancellationRequested)
          return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        float smooth = t * t * (3f - 2f * t); // smoothstep — sounds more natural than linear
        source.volume = Mathf.Lerp(from, to, smooth);

        await UniTask.NextFrame(ct);
      }

      if (!ct.IsCancellationRequested)
        source.volume = to;
    }
  }
}
