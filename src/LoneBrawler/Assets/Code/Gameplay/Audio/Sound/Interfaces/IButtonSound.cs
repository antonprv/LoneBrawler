// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using R3;

using UnityEngine.EventSystems;

namespace Code.Gameplay.Audio.Sound.Interfaces
{
  public interface IButtonSound
  {
    Observable<Unit> OnClickSoundFinished { get; }

    void OnPointerClick(PointerEventData eventData);
    void OnPointerEnter(PointerEventData eventData);
  }
}
