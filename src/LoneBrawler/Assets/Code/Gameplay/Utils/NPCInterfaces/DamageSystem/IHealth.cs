// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Animations;

using R3;

namespace Code.Gameplay.Utils.NPCInterfaces.DamageSystem
{
  public interface IHealth
  {
    public void Construct(IAnimator animator);

    ReadOnlyReactiveProperty<float> CurrentHealthRP { get; }
    ReadOnlyReactiveProperty<float> MaxHealthRP { get; }

    void TakeDamage(float damage);
  }
}
