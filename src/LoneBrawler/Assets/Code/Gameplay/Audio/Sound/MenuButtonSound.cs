// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Gameplay.Audio.Sound.Interfaces;

using Code.Gameplay.Audio.Sound.Types;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Gameplay.Audio.Sound
{
  public class MenuButtonSound :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    IButtonSound
  {
    public Button button;
    public SoundPlayer player;

    private readonly Subject<Unit> _onClickSoundFinished = new();
    public Observable<Unit> OnClickSoundFinished => _onClickSoundFinished;

    private CancellationToken _ctx;
    private bool _wasHovered;
    private bool _wasPressed;

    private void Awake() => _ctx = this.GetCancellationTokenOnDestroy();

    private void OnDestroy() => _onClickSoundFinished.OnCompleted();

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (button == null || !button.interactable)
        return;

      PlayHoverSound().Forget();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
      if (button == null || !button.interactable)
        return;

      PlayClickSound().Forget();
    }

    private async UniTaskVoid PlayHoverSound()
    {
      if (_ctx.IsCancellationRequested) return;

      if (_wasHovered) return;

      _wasHovered = true;

      await player.PlaySound(SoundType.MenuHover);

      _wasHovered = false;
    }

    private async UniTaskVoid PlayClickSound()
    {
      if (_ctx.IsCancellationRequested) return;

      if (_wasPressed) return;

      _wasPressed = true;

      await player.PlaySound(SoundType.MenuPressed);
      _onClickSoundFinished.OnNext(Unit.Default);

      _wasPressed = false;
    }
  }
}
