// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.Random;

using UnityEngine;

namespace Code.Gameplay.Audio.Sound.Types
{
  [System.Serializable]
  public class AudioClipGroup
  {
    public AudioClip[] clips;

    public AudioClip TryGetRandom(IRandomService random)
    {
      if (clips.Length == 0)
        return null;
      else if (clips.Length == 1)
        return clips[0];
      else
        return clips[random.Range(0, clips.Length, true)];
    }
  }
}
