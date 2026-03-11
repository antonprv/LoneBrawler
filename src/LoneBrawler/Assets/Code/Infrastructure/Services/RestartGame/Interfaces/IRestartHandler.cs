// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Sound.Types;

using R3;

namespace Code.Infrastructure.Services.RestartGame.Interfaces
{
  public interface IRestartHandler
  {
    public Observable<Unit> PrepareForRestart();
  }
}
