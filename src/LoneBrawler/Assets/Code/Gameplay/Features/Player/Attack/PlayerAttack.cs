// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.SaveData;
using Code.Data.SaveData.Player;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;
using Code.Common.DebugUtils;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Attack
{
  [RequireComponent(typeof(PlayerAnimator))]
  public class PlayerAttack : MonoBehaviour,
    IProgressReader, IProgressWriter, IPlayerAttacker, IActivatable
  {
    public int MaxHit
    {
      get => _stats.MaxEnemiesHit;
      set
      {
        if (value == _stats.MaxEnemiesHit) return;
        _stats.MaxEnemiesHit = value;
      }
    }

    public float Damage
    {
      get => _stats.Damage;
      set
      {
        if (value == _stats.Damage) return;
        _stats.Damage = value;
      }
    }

    public float Range
    {
      get => _stats.Range;
      set
      {
        if (value == _stats.Range) return;
        _stats.Range = value;
      }
    }
    public float Radius
    {
      get => _stats.Radius;
      set
      {
        if (value == _stats.Radius) return;
        _stats.Radius = value;
      }
    }

    public event Action OnAttacking;
    public event Action OnAttackFinished;

    public bool enableDebug = true;

    public Color debugIdleColor = Color.aliceBlue;
    public Color debugHitColor = Color.rebeccaPurple;

    private IInputService _inputService;
    private ITimeService _timeService;
    private IAnimator _animator;
    private IBuildConfigSubservice _build;
    private Collider[] _hits;
    private int _layerMask;
    private PlayerStats _stats;

    private bool _hasHit;
    private bool _isActive;

    public void Construct(
      IInputService inputService,
      ITimeService timeService,
      IGameConfigSubservice gameConfig,
      IBuildConfigSubservice buildConfig,
      IAnimator animator
      )
    {
      _inputService = inputService;
      _timeService = timeService;
      _animator = animator;

      _build = buildConfig;
      _layerMask = gameConfig.EnemyHitableLayerBitmask;
    }

    private void Update()
    {
      if (_inputService.IsAttackButtonUp() && _isActive)
      {
        OnAttacking?.Invoke();
        _animator.PlayPointAttack();
      }
    }

    private void OnAttackNormalAnimHit()
    {
      if (IsInvalid()) return;

      _hasHit = Hit();
      if (_hasHit)
      {
        foreach (Collider hit in _hits)
        {
          hit?.transform?.parent?.parent
            ?.GetComponent<IHealth>()
            ?.TakeDamage(Damage);
        }
      }
    }

    private bool IsInvalid() =>
      !_isActive
      || _hits == null
      || _stats == null;

    private void OnAttackNormalAnimEnd()
    {
      OnAttackFinished?.Invoke();
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
          duration: _timeService.DeltaTime
          );
      }
    }

    private bool Hit()
    {
      if (!_isActive) return false;

      int hitCount = Physics.OverlapSphereNonAlloc(
        GetHitPosition(),
        Radius,
        _hits,
        _layerMask
        );

      return hitCount > 0;
    }

    private Vector3 GetHitPosition() => new Vector3(
        transform.position.x,
        transform.position.y + 0.5f,
        transform.position.z
        ) + transform.forward * Range;

    public void ReadProgress(GameProgress playerProgress)
    {
      _stats = playerProgress.PlayerStats;
      _hits = new Collider[_stats.MaxEnemiesHit];
      Activate();
    }

    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.PlayerStats.Damage = Damage;
      playerProgress.PlayerStats.Range = Range;
      playerProgress.PlayerStats.MaxEnemiesHit = MaxHit;
    }

    public void Activate() => _isActive = true;
    public void Deactivate() => _isActive = false;
  }
}
