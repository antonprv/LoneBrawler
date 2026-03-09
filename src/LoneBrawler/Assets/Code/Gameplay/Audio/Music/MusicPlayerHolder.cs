// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Music.Interfaces;

namespace Code.Gameplay.Audio.Music
{
  public class MusicPlayerHolder : IMusicPlayerHolder
  {
    public IMusicPlayer Current { get; private set; }

    public void Register(IMusicPlayer player) => Current = player;
    public void Unregister() => Current = null;
  }
}

