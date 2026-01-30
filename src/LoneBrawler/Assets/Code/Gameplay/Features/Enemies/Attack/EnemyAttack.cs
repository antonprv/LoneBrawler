// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Linq;

using Code.Common.DebugUtils;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class EnemyAttack : MonoBehaviour, IEnemyAttacker
  {
    public EnemyAnimator animator;

    public float Range { get; set; }
    public float Radius { get; set; }
    public float Damage { get; set; }
    public int MaxHit { get; set; }

    public float Cooldown { get; set; }
    public float TurnSpeed { get; set; }

    public bool enableDebug = true;
    public Color debugIdleColor = Color.blue;
    public Color debugHitColor = Color.red;

    private ITimeService _timeService;
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

    public event Action OnAttacking;
    public event Action OnAttackFinished;

    public void Construct(
      GameObject player,
      IDeath playerDeath,
      IHealth playerHealth,
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig
      )
    {
      _hits = new Collider[MaxHit];

      _timeService = RootContext.Resolve<ITimeService>();

      _player = player;
      _playerHealth = playerHealth;
      _playerDeath = playerDeath;

      _build = buildConfig;
      _layerMask = gameConfig.PlayerCollision;
    }

    public void Activate() => _isActive = true;

    public void Deactivate() => _isActive = false;

    private void OnPointAttackHit()
    {
      _hasHit = Hit(out Collider hit);
      if (_hasHit)
      {
        _playerHealth?.TakeDamage(Damage);
      }
    }

    private void OnAreaAttackHitMelee() { }

    private void OnPointAttackEnded() => EndAttack();

    private void OnAreaAttackEnded() => EndAttack();

    private void Update()
    {
      if (!_isActive) return;

      if (!CooldownIsUp())
        _currentCooldown -= _timeService.DeltaTime;

      TurnToPlayer();

      if (CanAttack())
        StartAttack();
    }

    private void OnRenderObject()
    {
      if (!enableDebug) return;

      if (_build.IsDevelopment())
      {
        DrawDebugRuntime.DrawTempWireSphere(
          center: GetHitPosition(),
          radius: Radius,
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
      animator.PlayPointAttack();

      _isAttacking = true;
    }

    private void TurnToPlayer()
    {
      if (!_shouldTurnToPlayer) return;

      Vector3 direction = _player.transform.position - transform.position;
      direction.y = 0f;

      if (direction.sqrMagnitude < Constants.KINDA_SMALL_NUMBER)
        return;

      transform.rotation = Quaternion.Slerp(
          transform.rotation,
          Quaternion.LookRotation(direction),
          TurnSpeed * _timeService.DeltaTime
      );
    }

    private bool Hit(out Collider hit)
    {
      int hitCount = Physics.OverlapSphereNonAlloc(
        GetHitPosition(),
        Radius,
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
        ) + transform.forward * Range;

    private void EndAttack()
    {
      _shouldTurnToPlayer = false;
      _isAttacking = false;
      _hasHit = false;

      _currentCooldown = Cooldown;

      OnAttackFinished?.Invoke();
    }

    private bool CanAttack() => !_isAttacking && CooldownIsUp();

    private bool CooldownIsUp() => _currentCooldown.IsNearlyZero();
  }
}
