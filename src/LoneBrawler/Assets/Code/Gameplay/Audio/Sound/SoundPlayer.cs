// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Threading;

using Code.Gameplay.Audio.Sound.Interfaces;
using Code.Gameplay.Audio.Sound.Types;
using Code.Infrastructure.Services.RestartGame.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Audio.Sound
{
  [RequireComponent(typeof(SoundPlayer))]
  public class SoundPlayer : ZenjexBehaviour
  {
    public SoundComponent soundComponent;

    private ISoundProvider _soundProvider;
    [Zenjex] private readonly IRestartGameService _restartGameService;

    private CancellationToken _cancellationToken;

    protected override void OnAwake()
    {
      base.OnAwake();

      _soundProvider = soundComponent
        .gameObject
        .GetComponent<ISoundProvider>();

      _cancellationToken = this.GetCancellationTokenOnDestroy();
    }

    public async UniTask PlaySound(SoundType type, Action onSoundFinished = null)
    {
      var subject = new Subject<Unit>();
      var sound = _soundProvider.GetSound(type);

      if (sound == null) return;

      if (soundComponent.SoundSources.TryGetValue(type, out AudioSource source))
      {
        source.clip = sound;
        soundComponent.PlaySound(type);
        await UniTask.WaitWhile(() => source.isPlaying, PlayerLoopTiming.Update, _cancellationToken);
        onSoundFinished?.Invoke();
      }
    }
  }
}
