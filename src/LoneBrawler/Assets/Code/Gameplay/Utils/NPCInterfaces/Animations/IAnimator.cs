// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Utils.NPCInterfaces.Animations
{
  public interface IAnimator
  {
    public void PlayHit();
    public void PlayDeath();
    public void PlayPointAttack();
  }
}
