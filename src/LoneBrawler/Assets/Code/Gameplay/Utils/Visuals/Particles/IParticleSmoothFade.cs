// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using R3;

namespace Code.Gameplay.Utils.Visuals.Particles
{
  public interface IParticleSmoothFade
  {
    void TriggerStop();
    Observable<Unit> OnStopped { get; }
  }
}
