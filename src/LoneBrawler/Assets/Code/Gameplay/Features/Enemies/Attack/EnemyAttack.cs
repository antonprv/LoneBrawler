// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Enemies.Attack
{
  /// <summary>
  /// Manages the attack cycle (cooldown → start → hit → end).
  /// The actual hit logic is delegated to IAttackBehaviour (Strategy pattern).
  /// </summary>
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyAttack : ZenjexBehaviour, IEnemyAttacker
  {
    // ──────────────────────────────────────────────
    //  Enemy parameters (from SetValues)
    // ──────────────────────────────────────────────
    private float _cooldown;
    private float _turnSpeed;
    private float _hitRecoverCooldown;

    // ──────────────────────────────────────────────
    //  Debug
    // ──────────────────────────────────────────────
    public bool enableDebug = true;
    public Color debugIdleColor = Color.blue;
    public Color debugHitColor = Color.red;

    // ──────────────────────────────────────────────
    //  Injected
    // ──────────────────────────────────────────────
    [Zenjex] private readonly ITimeService _timeService;

    // ──────────────────────────────────────────────
    //  Runtime state
    // ──────────────────────────────────────────────
    private IAnimator _animator;
    private IBuildConfigSubservice _build;
    private GameObject _player;
    private IDeath _playerDeath;

    private IAttackBehaviour _behaviour;   // strategy: Melee or Ranged

    private bool _isAttacking;
    private bool _isActive;
    private bool _shouldTurnToPlayer;
    private float _currentCooldown;

    private CompositeDisposable _disposables;

    public event Action OnAttacking;
    public event Action OnAttackFinished;

    // ──────────────────────────────────────────────
    //  IEnemyStaticDataReceiver
    // ──────────────────────────────────────────────
    public void SetValues(EnemyStaticData data)
    {
      _cooldown = data.AttackCooldown;
      _turnSpeed = data.AttackTurnSpeed;
      _hitRecoverCooldown = data.HitRecoverCooldown;
    }

    // ──────────────────────────────────────────────
    //  IEnemyAttacker
    // ──────────────────────────────────────────────
    public void Construct(
      GameObject player,
      IAnimator animator,
      IDeath playerDeath,
      IHealth playerHealth,
      IHealth enemyHealth,
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig)
    {
      _disposables = new CompositeDisposable();
      _animator = animator;
      _player = player;
      _playerDeath = playerDeath;
      _build = buildConfig;

      SubscribeToTakingDamage(enemyHealth);
    }

    /// <summary>
    /// Called by the factory after Construct - injects the attack strategy.
    /// </summary>
    public void SetAttackBehaviour(IAttackBehaviour behaviour) =>
      _behaviour = behaviour;

    public void StartAttacking() => _isActive = true;

    public void Deactivate() => _isActive = false;

    // ──────────────────────────────────────────────
    //  Unity lifecycle
    // ──────────────────────────────────────────────
    private void Update()
    {
      if (!_isActive) return;

      if (!CooldownIsUp())
        _currentCooldown -= _timeService.DeltaTime;

      TurnToPlayer();

      if (CanAttack())
        StartAttack();
    }

    private void OnDestroy() => _disposables?.Dispose();

    // Called from AnimationEvent
    private void OnPointAttackHit()
    {
      if (!_isActive) return;
      _behaviour?.PerformHit();
    }

    // Called from AnimationEvent at the moment of projectile release
    private void OnRangedAttackCast() =>
      _behaviour?.OnCast();

    private void OnPointAttackEnded() => EndAttack();
    private void OnAreaAttackEnded() => EndAttack();

    // ──────────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────────
    private void SubscribeToTakingDamage(IHealth enemyHealth)
    {
      enemyHealth.CurrentHealthRP
        .Skip(1)
        .Subscribe(_ => StartCoroutine(RecoverAfterHit()))
        .AddTo(_disposables);
    }

    private IEnumerator RecoverAfterHit()
    {
      yield return new WaitForSeconds(_hitRecoverCooldown);
      if (_isAttacking) EndAttack();
    }

    private void StartAttack()
    {
      if (_playerDeath.IsDead) return;

      _shouldTurnToPlayer = true;
      _isAttacking = true;

      OnAttacking?.Invoke();
      _animator.PlayPointAttack();
    }

    private void EndAttack()
    {
      _behaviour?.OnAttackEnded();

      _shouldTurnToPlayer = false;
      _isAttacking = false;
      _currentCooldown = _cooldown;

      OnAttackFinished?.Invoke();
    }

    private void TurnToPlayer()
    {
      if (!_shouldTurnToPlayer) return;

      Vector3 direction = _player.transform.position - transform.position;
      direction.y = 0f;

      if (direction.sqrMagnitude < FMath.KINDA_SMALL_NUMBER) return;

      transform.rotation = Quaternion.Slerp(
        transform.rotation,
        Quaternion.LookRotation(direction),
        _turnSpeed * _timeService.DeltaTime);
    }

    private bool CanAttack() => !_isAttacking && CooldownIsUp();
    private bool CooldownIsUp() => _currentCooldown.IsNearlyZero();
  }
}
