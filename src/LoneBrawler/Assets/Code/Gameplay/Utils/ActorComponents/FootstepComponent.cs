// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Gameplay.Utils.ActorComponents
{
  public class FootstepComponent : MonoBehaviour
  {
    public SoundPlayer soundPlayer;

    private void OnStep() => soundPlayer.PlaySound(SoundType.Footstep).Forget();
  }
}
