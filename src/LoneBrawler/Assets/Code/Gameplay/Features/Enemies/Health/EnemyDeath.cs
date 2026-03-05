// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

using R3;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Gameplay.Features.Enemies.Health
{
  public class EnemyDeath : MonoBehaviour, IEnemyDeath
  {
    public GameObject DeathFX;
    public Vector3 fXOffset = new(0f, 0.01f, 0f);

    public bool IsDead { get; private set; }
    private float _disappearDelay;

    private IGameLog _logger;
    private IAnimator _animator;
    private IHealth _health;
    private GameObject _spawnedDeathFX;
    private CompositeDisposable _disposables;

    public event Action OnDead;

    public void SetValues(EnemyStaticData staticData)
    {
      _disappearDelay = staticData.DisappearDelay;
    }

    public void Construct(IAnimator animator, IHealth health)
    {
      IsDead = false;
      _logger = RootContext.Resolve<IGameLog>();
      _animator = animator;
      _health = health;
      _disposables = new CompositeDisposable();

      SubscribeToRP();
    }

    private void SubscribeToRP()
    {
      _health.CurrentHealthRP
        .Skip(1)
        .Where(hp => hp.IsNearlyZero())
        .Subscribe(_ => Die())
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables?.Dispose();

    private void Die()
    {
      OnDead?.Invoke();
      DeactivateComponents();

      if (_animator != null)
        _animator.PlayDeath();

      _spawnedDeathFX = Instantiate(
        DeathFX,
        transform.position + fXOffset,
        Quaternion.identity
        );

      IsDead = true;

      StartCoroutine(DespawnEnemy());
    }

    private IEnumerator DespawnEnemy()
    {
      yield return new WaitForSeconds(_disappearDelay);

      _logger.Log("Destroying enemy...");
      Destroy(_spawnedDeathFX);
      Destroy(gameObject);
    }

    private void DeactivateComponents()
    {
      foreach (var component in gameObject.GetComponents<IDeactivatable>())
        component.Deactivate();
    }
  }
}
