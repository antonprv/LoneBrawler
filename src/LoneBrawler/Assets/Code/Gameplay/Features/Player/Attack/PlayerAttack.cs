// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.DebugUtils;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.SaveData;
using Code.Data.SaveData.Player;
using Code.Data.StaticData.Configs.BuildConfig;
using Code.Gameplay.Common.NPCInterfaces;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Player.Animations;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Attack
{
  [RequireComponent(typeof(PlayerAnimator))]
  public class PlayerAttack : MonoBehaviour,
    IProgressReader, IProgressWriter, IAttacker, IActivatable, IConstructableComponent
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

    public float AttackRange
    {
      get => _stats.Range;
      set
      {
        if (value == _stats.Range) return;
        _stats.Range = value;
      }
    }
    public float AttackRadius
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

    public PlayerAnimator animator;

    public bool enableDebug = true;

    public Color debugIdleColor = Color.aliceBlue;
    public Color debugHitColor = Color.rebeccaPurple;

    private IInputService _inputService;
    private ITimeService _timeService;
    private IStaticDataService _staticDataService;
    private Collider[] _hits;
    private int _layerMask;
    private PlayerStats _stats;

    private bool _hasHit;
    private bool _isActive;

    public void Initialize()
    {
      _hits = new Collider[_stats.MaxEnemiesHit];
      Activate();
    }

    private void Awake()
    {
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();

      _layerMask = _staticDataService.GameConfig.EnemyHitableLayer;
    }

    private void Update()
    {
      if (_inputService.IsAttackButtonUp() && _isActive)
      {
        OnAttacking?.Invoke();
        animator.PlayAttack();
      }
    }

    private void OnAttackNormalAnimHit()
    {
      _hasHit = Hit();
      if (_hasHit)
      {
        foreach (Collider hit in _hits)
        {
          hit?.transform.parent.parent.GetComponent<IHealth>().TakeDamage(Damage);
        }
      }
    }

    private void OnAttackNormalAnimEnd()
    {
      OnAttackFinished?.Invoke();
    }

    private void OnRenderObject()
    {
      if (!enableDebug) return;

      if (CurrentBuild.GetConfiguration() == BuildConfiguration.Development)
      {
        DrawDebugRuntime.DrawTempWireSphere(
          center: GetHitPosition(),
          radius: AttackRadius,
          color: _hasHit ? debugHitColor : debugIdleColor,
          segments: 12,
          duration: _timeService.DeltaTime
          );
      }
    }

    private bool Hit()
    {
      int hitCount = Physics.OverlapSphereNonAlloc(
        GetHitPosition(),
        AttackRadius,
        _hits,
        _layerMask
        );

      return hitCount > 0;
    }

    private Vector3 GetHitPosition() => new Vector3(
        transform.position.x,
        transform.position.y + 0.5f,
        transform.position.z
        ) + transform.forward * AttackRange;

    public void ReadProgress(GameProgress playerProgress) =>
      _stats = playerProgress.PlayerStats;

    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.PlayerStats.Damage = Damage;
      playerProgress.PlayerStats.Range = AttackRange;
      playerProgress.PlayerStats.MaxEnemiesHit = MaxHit;
    }

    public void Activate() => _isActive = true;
    public void Deactivate() => _isActive = false;
  }
}
