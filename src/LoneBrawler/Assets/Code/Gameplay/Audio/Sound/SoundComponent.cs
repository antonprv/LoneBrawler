// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Sound.Types;

using Code.Common.CustomTypes.Domain.Collections;
using Code.Infrastructure.Services.SoundService.Interfaces;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Audio.Sound
{
  public class SoundComponent : ZenjexBehaviour
  {
    public DictionaryData<SoundType, AudioSource> SoundSources = new();

    [Zenjex] private readonly ISoundService _soundService;

    private readonly CompositeDisposable _disposables = new();

    public void Construct() => SubscribeToRP();

    private void SubscribeToRP()
    {
      _soundService.SoundVolumeRP
        .Skip(1)
        .Subscribe(x =>
        {
          foreach (var sound in SoundSources)
            sound.Value.volume = x;
        })
        .AddTo(_disposables);
    }

    public void PlaySound(SoundType soundType)
    {
      if (SoundSources.TryGetValue(soundType, out AudioSource sound))
        sound.Play();
    }

    public void StopSound(SoundType soundType)
    {
      if (SoundSources.TryGetValue(soundType, out AudioSource sound))
        sound.Stop();
    }

    private void OnDestroy() => _disposables.Dispose();
  }
}
