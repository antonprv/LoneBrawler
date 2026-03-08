// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Sound.Types;

using UnityEngine;

namespace Code.Gameplay.Audio.Sound.Interfaces
{
  public interface ISoundProvider
  {
    AudioClip GetSound(SoundType soundType);
  }
}
