// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Sound.Interfaces;
using Code.Gameplay.Audio.Sound.Types;

using UnityEngine;

namespace Code.Gameplay.Audio.Sound
{
  [RequireComponent(typeof(SoundPlayer))]
  public class SoundPlayer : MonoBehaviour
  {
    public SoundComponent soundComponent;

    private ISoundProvider _soundProvider;
    private AudioSource _audioSource;

    private void Awake()
    {
      _soundProvider = soundComponent
        .gameObject
        .GetComponent<ISoundProvider>();

      soundComponent
        .SoundSources
        .TryGetValue(SoundType.Footstep, out _audioSource);
    }

    public void PlaySound(SoundType type)
    {
      var sound = _soundProvider.GetSound(type);
      if (sound == null) return;

      _audioSource.generator = sound;
      soundComponent.PlaySound(type);
    }
  }
}
