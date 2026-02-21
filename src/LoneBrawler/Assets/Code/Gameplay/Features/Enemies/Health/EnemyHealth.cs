// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using R3;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyHealth : MonoBehaviour, IHealth, IEnemyStaticDataReceiver
  {
    private ReactiveProperty<float> _currentHealthRP = new(0f);
    private ReactiveProperty<float> _maxHealthRP = new(0f);

    public ReadOnlyReactiveProperty<float> CurrentHealthRP => _currentHealthRP;
    public ReadOnlyReactiveProperty<float> MaxHealthRP => _maxHealthRP;

    private IAnimator _animator;

    public void SetValues(EnemyStaticData staticData)
    {
      _maxHealthRP.Value = staticData.MaxHealth;
      _currentHealthRP.Value = staticData.MaxHealth;
    }

    public void Construct(IAnimator animator) => _animator = animator;

    public void TakeDamage(float damage)
    {
      if (_currentHealthRP.Value.IsNearlyZero()) return;

      _currentHealthRP.Value -= damage;
      _animator.PlayHit();
    }

    private void OnDestroy()
    {
      _currentHealthRP.Dispose();
      _maxHealthRP.Dispose();
    }
  }
}
