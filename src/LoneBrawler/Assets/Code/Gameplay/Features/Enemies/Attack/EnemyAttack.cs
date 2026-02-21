// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Code.Common.DebugUtils;
using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Animations;
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
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyAttack : ZenjexBehaviour, IEnemyAttacker
  {
    private float _range;
    private float _radius;
    private float _damage;
    private int _maxHit;

    private float _cooldown;
    private float _turnSpeed;

    public bool enableDebug = true;
    public Color debugIdleColor = Color.blue;
    public Color debugHitColor = Color.red;

    [Zenjex] private readonly ITimeService _timeService;

    private IAnimator _animator;
    private IBuildConfigSubservice _build;

    private GameObject _player;

    private IHealth _playerHealth;
    private IDeath _playerDeath;
    private Collider[] _hits;
    private int _layerMask;

    private bool _isAttacking = false;
    private bool _hasHit = false;
    private bool _isActive = false;
    private bool _shouldTurnToPlayer;
    private float _currentCooldown;
    private float _hitRecoverCooldown;
    private CompositeDisposable _disposables;

    public event Action OnAttacking;
    public event Action OnAttackFinished;

    public void SetValues(EnemyStaticData staticData)
    {
      _range = staticData.AttackRange;
      _radius = staticData.AttackRadius;
      _damage = staticData.AttackDamage;
      _maxHit = staticData.AttackMaxHit;
      _cooldown = staticData.AttackCooldown;
      _turnSpeed = staticData.AttackTurnSpeed;
      _hitRecoverCooldown = staticData.HitRecoverCooldown;
    }

    public void Construct(
      GameObject player,
      IAnimator animator,
      IDeath playerDeath,
      IHealth playerHealth,
      IHealth enemyHealth,
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig
      )
    {
      _hits = new Collider[_maxHit];
      _disposables = new CompositeDisposable();

      _animator = animator;

      _player = player;
      _playerHealth = playerHealth;
      _playerDeath = playerDeath;

      SubscribeToTakingDamage(enemyHealth);

      _build = buildConfig;
      _layerMask = gameConfig.PlayerLayerBitmask;
    }

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
      if (_isAttacking)
        EndAttack();
    }

    public void StartAttacking() => _isActive = true;

    public void Deactivate() => _isActive = false;

    private void Update()
    {
      if (!_isActive) return;

      if (!CooldownIsUp())
        _currentCooldown -= _timeService.DeltaTime;

      TurnToPlayer();

      if (CanAttack())
        StartAttack();
    }

    private void OnDestroy() => _disposables.Dispose();

    private void OnPointAttackHit()
    {
      if (IsInvalid()) return;

      _hasHit = Hit(out Collider hit);
      if (_hasHit)
      {
        _playerHealth?.TakeDamage(_damage);
      }
    }

    private bool IsInvalid() =>
      !_isActive
      || _hits == null;

    private void OnAreaAttackHitMelee() { }

    private void OnPointAttackEnded() => EndAttack();

    private void OnAreaAttackEnded() => EndAttack();



    private void OnRenderObject()
    {
      if (!enableDebug) return;

      if (_build.IsDevelopment())
      {
        DrawDebugRuntime.DrawTempWireSphere(
          center: GetHitPosition(),
          radius: _radius,
          color: _hasHit ? debugHitColor : debugIdleColor,
          segments: 12,
          duration: _timeService.DeltaAtOffset
          );
      }
    }

    private bool IsPlayerDead() => _playerDeath.IsDead;

    private void StartAttack()
    {
      if (IsPlayerDead()) return;

      _shouldTurnToPlayer = true;

      OnAttacking?.Invoke();
      _animator.PlayPointAttack();

      _isAttacking = true;
    }

    private void TurnToPlayer()
    {
      if (!_shouldTurnToPlayer) return;

      Vector3 direction = _player.transform.position - transform.position;
      direction.y = 0f;

      if (direction.sqrMagnitude < FMath.KINDA_SMALL_NUMBER)
        return;

      transform.rotation = Quaternion.Slerp(
          transform.rotation,
          Quaternion.LookRotation(direction),
          _turnSpeed * _timeService.DeltaTime
      );
    }

    private bool Hit(out Collider hit)
    {
      int hitCount = Physics.OverlapSphereNonAlloc(
        GetHitPosition(),
        _radius,
        _hits,
        _layerMask
        );

      hit = _hits.FirstOrDefault();

      return hitCount > 0;
    }

    private Vector3 GetHitPosition() => new Vector3(
        transform.position.x,
        transform.position.y + 0.5f,
        transform.position.z
        ) + transform.forward * _range;

    private void EndAttack()
    {
      _shouldTurnToPlayer = false;
      _isAttacking = false;
      _hasHit = false;

      _currentCooldown = _cooldown;

      OnAttackFinished?.Invoke();
    }

    private bool CanAttack() => !_isAttacking && CooldownIsUp();

    private bool CooldownIsUp() => _currentCooldown.IsNearlyZero();
  }
}
