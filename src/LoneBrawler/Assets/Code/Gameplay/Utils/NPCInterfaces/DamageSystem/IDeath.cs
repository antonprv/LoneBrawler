// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Animations;

namespace Code.Gameplay.Utils.NPCInterfaces.DamageSystem
{
  public interface IDeath
  {
    public bool IsDead { get; }
    public void Construct(IAnimator animator, IHealth health);
  }
}
