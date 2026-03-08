// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Logging;
using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;
using UnityEngine.AddressableAssets;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Enemies.Health
{
  public class EnemyDeath : ZenjexBehaviour, IEnemyDeath
  {
    public SoundPlayer soundPlayer;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly IAssetLoader _assetLoader;

    public bool IsDead { get; private set; }
    private float _disappearDelay;
    private AssetReferenceGameObject _deathPrefab;
    private Vector3 _deathFXSpawnOffset;

    private IAnimator _animator;
    private IHealth _health;
    private CompositeDisposable _disposables = new();

    private GameObject _spawnedDeathFX;

    private readonly Subject<Unit> _onDead = new();
    public Observable<Unit> OnDead => _onDead;

    public void SetValues(EnemyStaticData staticData)
    {
      _disappearDelay = staticData.DisappearDelay;
      _deathPrefab = staticData.DeathFXPrefabReference;
      _deathFXSpawnOffset = staticData.DeathFXSpawnOffset;
    }

    public void Construct(IAnimator animator, IHealth health)
    {
      IsDead = false;
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
        .Subscribe(_ => Die().Forget())
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables?.Dispose();

    private async UniTaskVoid Die()
    {
      _onDead.OnNext(Unit.Default);

      DeactivateComponents();

      _animator.PlayDeath();
      soundPlayer.PlaySound(SoundType.Death);

      await SpawnFX();

      IsDead = true;

      DespawnEnemy().Forget();
    }

    private async UniTask SpawnFX()
    {
      _spawnedDeathFX =
        await _assetLoader
        .InstantiateAsync(_deathPrefab, gameObject.transform);

      if (_spawnedDeathFX != null)
      {
        _spawnedDeathFX.transform.position =
          transform.position + _deathFXSpawnOffset;
      }
      else
      {
        _logger.
          Log(LogType.Error,
          $"{nameof(_deathPrefab)} is invalid or missing");
      }
    }

    private async UniTaskVoid DespawnEnemy()
    {
      await UniTask.Delay(
          TimeSpan.FromSeconds(_disappearDelay),
          cancellationToken: this.GetCancellationTokenOnDestroy());

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
