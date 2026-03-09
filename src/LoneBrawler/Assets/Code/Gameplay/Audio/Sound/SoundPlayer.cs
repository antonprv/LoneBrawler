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

    private void Awake()
    {
      _soundProvider = soundComponent
        .gameObject
        .GetComponent<ISoundProvider>();
    }

    public void PlaySound(SoundType type)
    {
      var sound = _soundProvider.GetSound(type);
      if (sound == null) return;

      if (soundComponent.SoundSources.TryGetValue(type, out AudioSource source))
      {
        source.clip = sound;
        soundComponent.PlaySound(type);
      }
    }
  }
}
