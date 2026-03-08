// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx.Interfaces;

using NSubstitute;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx
{
  /// <summary>
  /// Attached to each pooled VFX object.
  /// Auto-returns itself to the pool after <see cref="Duration"/> seconds.
  /// </summary>
  public class VfxInstance : MonoBehaviour
  {
    [SerializeField] private float _duration = 2f;

    private IVfxPool _pool;
    private float _timeAlive;
    private bool _active;

    public void Play(IVfxPool pool, float duration)
    {
      _pool = pool;
      _duration = duration;
      _timeAlive = 0f;
      _active = true;
    }

    private void Update()
    {
      if (!_active) return;

      _timeAlive += Time.deltaTime;
      if (_timeAlive >= _duration)
        ReturnToPool();
    }

    private void OnDisable() => _active = false;

    private void ReturnToPool()
    {
      _active = false;
      _pool?.Return(this);
    }
  }
}
