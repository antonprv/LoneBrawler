// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Audio.Music.Interfaces
{
  /// <summary>
  /// Smoothly transitions the volume of an AudioSource from one value to another.
  /// Responsible only for the math and timing of volume interpolation.
  /// </summary>
  public interface IFader
  {
    /// <param name="source">The AudioSource whose volume is being adjusted.</param>
    /// <param name="from">Starting volume (0–1).</param>
    /// <param name="to">Target volume (0–1).</param>
    /// <param name="duration">Duration of the fade in seconds. Zero applies the target instantly.</param>
    /// <param name="ct">Token that cancels the fade mid-way without snapping to the target.</param>
    UniTask Fade(AudioSource source, float from, float to, float duration, CancellationToken ct);
  }
}
