// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Linq;

using Code.Common.DebugUtils;
using Code.Common.Extensions.ReflexExtensions;
using Code.Configs;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Enemies.Animations;
using Code.Gameplay.Features.Player;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class Attack : MonoBehaviour
  {
    public EnemyAnimator animator;

    public float attackCooldown = 0.3f;
    public float hitRadius = 0.7f;
    public float hitRange = 0.8f;
    public float attackTurnSpeed = 5f;
    public float attackDamage = 10f;

    private Color debugIdleColor = Color.blue;
    private Color debugHitColor = Color.red;

    private IPlayerReader _playerReader;
    private ITimeService _timeService;
    private GameObject _player;
    private PlayerHealth _playerHealth;
    private PlayerDeath _playerDeath;

    private Collider[] _hits = new Collider[1];
    private int _layerMask;

    private bool _isAttacking = false;
    private bool _wasHit = false;
    private bool _isActive = false;
    private bool _shouldTurnToPlayer;
    private float _currentCooldown;

    private void Awake()
    {
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _timeService = RootContext.Resolve<ITimeService>();

      _layerMask = GameConfiguration.PlayerCollision;
    }

    public void EnableAttack() => _isActive = true;

    public void DisableAttack() => _isActive = false;

    private void OnPointAttackHit()
    {
      _wasHit = Hit(out Collider hit);
      if (_wasHit && IsPlayerValid())
      {
        _playerHealth?.TakeDamage(attackDamage);
      }
    }

    private void OnAreaAttackHitMelee() { }

    private void OnPointAttackEnded() => EndAttack();

    private void OnAreaAttackEnded() => EndAttack();

    private void Update()
    {
      if (!GetPlayer()) return;

      if (!CooldownIsUp())
        _currentCooldown -= _timeService.DeltaTime;

      TurnToPlayer();

      if (CanAttack())
        StartAttack();
    }

    private bool GetPlayer()
    {
      if (!IsPlayerValid())
      {
        _player = _playerReader.Player;
        _playerHealth = _player?.GetComponent<PlayerHealth>();
        _playerDeath = _player?.GetComponent<PlayerDeath>();

        return false;
      }

      return true;
    }

    private void OnRenderObject()
    {
      if (CurrentBuild.GetConfiguration() == BuildConfiguration.Development)
      {
        DrawDebugRuntime.DrawTempWireSphere(
          center: GetHitPosition(),
          radius: hitRadius,
          color: _wasHit ? debugHitColor : debugIdleColor,
          segments: 16,
          duration: _timeService.DeltaAtOffset
          );
      }
    }

    private bool IsPlayerValid() => _player != null;

    private bool IsPlayerDead() => _playerDeath == null || _playerDeath.IsDead;

    private void StartAttack()
    {
      if (IsPlayerDead()) return;

      _shouldTurnToPlayer = true;

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
          attackTurnSpeed * _timeService.DeltaTime
      );
    }

    private bool Hit(out Collider hit)
    {
      int hitCount = Physics.OverlapSphereNonAlloc(
        GetHitPosition(),
        hitRadius,
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
        ) + transform.forward * hitRange;

    private void EndAttack()
    {
      _shouldTurnToPlayer = false;
      _isAttacking = false;
      _wasHit = false;

      _currentCooldown = attackCooldown;
    }

    private bool CanAttack() => !_isAttacking && CooldownIsUp() && _isActive;

    private bool CooldownIsUp() => _currentCooldown.IsNearlyZero();
  }
}
