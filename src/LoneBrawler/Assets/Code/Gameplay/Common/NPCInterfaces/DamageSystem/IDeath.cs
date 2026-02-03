// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.NPCInterfaces.Animations;

namespace Code.Gameplay.Common.NPCInterfaces.DamageSystem
{
  public interface IDeath
  {
    public bool IsDead { get; }
    public void Construct(IAnimator animator, IHealth health);
  }
}
