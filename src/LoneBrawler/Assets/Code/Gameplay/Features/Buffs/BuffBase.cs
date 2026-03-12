// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;
using System.Threading;

using Code.Common.Extensions.Async;
using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs
{
  public class BuffBase
  {
    #region Public Fields

    public ReadOnlyReactiveProperty<BuffState> BuffStateRP => _buffStateRP;
    public BuffClassName ClassName { get; private set; }
    public BuffActivationType ActivationType { get; private set; }
    // Public access is needed for BuffTrackerService to take a snapshot.
    public float RemainingDuration => _buffDuration;

    #endregion

    #region Protected Fields

    protected GameObject BuffOwner { get; private set; }
    protected Transform BuffOwnerTransform { get; private set; }
    protected float TotalDuration { get; private set; }

    // Spawned visual effect. What to do with it is decided by the specific buff.
    protected GameObject SpawnedEffect { get; private set; }

    #endregion

    #region Private Fields

    private float _buffDuration;
    private Coroutine _durationCoroutine;
    private readonly ReactiveProperty<BuffState> _buffStateRP = new();

    private readonly ICoroutineRunner _runner;
    protected readonly ITimeService Time;
    private readonly IAssetLoader _assetLoader;
    private readonly BuffStaticData _buffStaticData;

    private IBuffTrackerService _buffTracker;

    private bool _isCleanedUp;

    #endregion

    #region Constructor

    public BuffBase(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      IBuffDataSubservice dataSubservice,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      )
    {
      if (buffStaticData.Class == BuffClassName.None)
        throw new InvalidOperationException(
          "[BuffBase] BuffStaticData.Class = None - this is not allowed.");

      if (buffStaticData.Class == BuffClassName.BuffBase)
        throw new InvalidOperationException(
          "[BuffBase] Cannot create an instance of BuffBase directly. " +
          "BuffBase is an abstract base type, use a concrete subclass.");

      _runner = coroutineRunner;
      Time = timeService;
      _assetLoader = assetLoader;
      _buffStaticData = buffStaticData;

      BuffOwner = buffOwner;
      BuffOwnerTransform = buffOwner.transform;

      ClassName = buffStaticData.Class;
      ActivationType = buffStaticData.ActivationType;

      _buffDuration = buffStaticData.Duration;
      TotalDuration = buffStaticData.Duration;

      _buffStateRP.Value = BuffState.Passive;
    }

    /// <summary>
    /// Protected constructor for tests.
    /// Allows creating minimal doubles without DI dependencies.
    /// </summary>
    protected BuffBase(BuffClassName className, BuffActivationType activationType, BuffState initialState)
    {
      ClassName = className;
      ActivationType = activationType;
      _buffStateRP = new ReactiveProperty<BuffState>(initialState);
      _buffDuration = 10f;
    }

    #endregion

    #region Overridable methods

    protected virtual void BurstActivation() { }
    protected virtual void ConstantActivation() { }

    // Called once when a Duration-buff starts, before the first tick.
    protected virtual void OnDurationStarted() { }

    // Called every frame while the buff is active.
    protected virtual void OnDurationTick() { }

    // Called once when a Duration-buff ends.
    protected virtual void OnDurationEnded() { }

    // Called when a Constant-buff is restored from a save.
    // Stat-effect is already recorded in PlayerStats, override only for visual effects.
    protected virtual void OnConstantRestored() { }

    #endregion

    #region Public API

    public void RegisterTracker(IBuffTrackerService buffTracker) =>
      _buffTracker = buffTracker;

    public void Activate()
    {
      switch (ActivationType)
      {
        case BuffActivationType.None:
          break;

        case BuffActivationType.Burst:
          RunBurstActivation();
          break;

        case BuffActivationType.Constant:
          RunConstantActivation();
          break;

        case BuffActivationType.Duration:
          _runner.StartCoroutine(RunDurationActivation());
          break;

        default:
          break;
      }
    }

    /// <summary>
    /// Sets the remaining time before calling Activate().
    /// Needed for restoring Duration-buffs from saves with exact remainder.
    /// </summary>
    public void SetRemainingDuration(float remainingDuration)
    {
      _buffDuration = Mathf.Max(0f, remainingDuration);
    }

    /// <summary>
    /// Restores a Constant-buff from a save.
    /// Marks the buff as active but DOES NOT reapply stat-effects -
    /// they are already stored in saved PlayerStats.
    /// Calls OnConstantRestored() for subclasses that need to restore visuals.
    /// </summary>
    public void RestoreConstantBuff()
    {
      _buffStateRP.Value = BuffState.Active;
      OnConstantRestored();
    }

    #endregion

    #region Managing visual effect

    /// <summary>
    /// Instantiates the prefab of the visual effect via Addressables.
    /// Result is saved into SpawnedEffect.
    /// </summary>
    protected async UniTask SpawnEffectAsync(Transform parent = null, CancellationToken ct = default)
    {
      if (_buffStaticData.BuffEffectPrefab == null ||
          string.IsNullOrEmpty(_buffStaticData.BuffEffectPrefab.AssetGUID))
        return;

      DestroyEffect(); // Avoid visual effect stacking

      SpawnedEffect = await _assetLoader.InstantiateAsync(
        _buffStaticData.BuffEffectPrefab,
        parent
        );

      // Discard the result if the owner was destroyed while we were loading.
      if (ct.IsCancellationRequested || _isCleanedUp)
      {
        DestroyEffect();
      }
    }

    /// <summary>
    /// Destroys the spawned visual effect.
    /// </summary>
    protected void DestroyEffect()
    {
      if (SpawnedEffect != null)
        GameObject.Destroy(SpawnedEffect);

      SpawnedEffect = null;
    }

    public void Cleanup()
    {
      _isCleanedUp = true;

      DestroyEffect();

      if (_durationCoroutine != null)
      {
        _runner.StopCoroutine(_durationCoroutine);
        _durationCoroutine = null;
      }
    }

    #endregion

    #region Private activation mechanics

    private void RunBurstActivation()
    {
      _buffStateRP.Value = BuffState.Active;
      BurstActivation();
      _buffStateRP.Value = BuffState.Disabled;
      _buffTracker.RemoveBuff(this, ClassName);
    }

    private void RunConstantActivation()
    {
      _buffStateRP.Value = BuffState.Active;
      ConstantActivation();
    }

    private IEnumerator RunDurationActivation()
    {
      if (_durationCoroutine != null)
      {
        _runner.StopCoroutine(_durationCoroutine);
        _durationCoroutine = null;
        OnDurationEnded();
      }

      _buffDuration = TotalDuration;
      _durationCoroutine = _runner.StartCoroutine(DurationRoutine());
      yield break;
    }

    private IEnumerator DurationRoutine()
    {
      _buffStateRP.Value = BuffState.Active;
      OnDurationStarted();

      while (_buffDuration > FMath.KINDA_SMALL_NUMBER)
      {
        OnDurationTick();
        _buffDuration -= Time.UnscaledDeltaTime;
        yield return null;
      }

      OnDurationEnded();
      _buffStateRP.Value = BuffState.Disabled;
      _durationCoroutine = null;
      _buffTracker.RemoveBuff(this, ClassName);
    }

    #endregion
  }
}
