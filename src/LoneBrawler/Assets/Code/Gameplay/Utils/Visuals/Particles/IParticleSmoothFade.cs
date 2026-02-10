// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Utils.Visuals.Particles
{
  public interface IParticleSmoothFade
  {
    public void TriggerStop();
    public event Action OnStopped;
  }
}
