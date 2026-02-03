// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Common.Visuals.Particles
{
  public interface IParticleSmoothFade
  {
    public void TriggerStop();
    public event Action OnStopped;
  }
}
