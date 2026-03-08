// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Audio.Sound.Interfaces
{
  public interface ISoundComponent
  {
    void Construct();
    void PlaySound();
    void StopSound();
  }
}
