// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Gameplay.Audio.Sound.Interfaces;
using Code.Gameplay.Audio.Sound.Types;
using Code.Infrastructure.Services.Random;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Audio.Sound
{
  public class SoundList : ZenjexBehaviour, ISoundProvider
  {
    public DictionaryData<SoundType, AudioClipGroup> soundClips;

    [Zenjex] private readonly IRandomService _random;

    public AudioClip GetSound(SoundType soundType)
    {
      if (soundClips.TryGetValue(soundType, out AudioClipGroup group))
        return group.TryGetRandom(_random);
      return null;
    }
  }
}
