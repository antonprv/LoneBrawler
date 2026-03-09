// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Audio.Music.Interfaces
{
  public interface IMusicPlayerHolder
  {
    IMusicPlayer Current { get; }
    void Register(IMusicPlayer player);
    void Unregister();
  }
}
