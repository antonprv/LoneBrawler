// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Health
{
  public class EnemyHealth : MonoBehaviour, IHealth, IEnemyStaticDataReceiver
  {
    public SoundPlayer soundPlayer;

    public ReadOnlyReactiveProperty<float> CurrentHealthRP => _currentHealthRP;
    public ReadOnlyReactiveProperty<float> MaxHealthRP => _maxHealthRP;

    private ReactiveProperty<float> _currentHealthRP = new(0f);
    private ReactiveProperty<float> _maxHealthRP = new(0f);

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

      if (_animator != null)
        _animator.PlayHit();

      if (soundPlayer != null)
        soundPlayer.PlaySound(SoundType.Hit).Forget();
    }

    private void OnDestroy()
    {
      _currentHealthRP.Dispose();
      _maxHealthRP.Dispose();
    }
  }
}
