// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.Async;
using Code.Common.FastMath;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

namespace Code.Gameplay.Features.Buffs
{
  public class BuffBase
  {
    private readonly ReactiveProperty<BuffState> _buffStateRP = new();
    public ReadOnlyReactiveProperty<BuffState> BuffStateRP => _buffStateRP;

    public BuffClassName ClassName { get; private set; }
    public BuffActivationType ActivationType { get; private set; }

    protected GameObject BuffOwner { get; private set; }
    protected Transform BuffOwnerTransform { get; private set; }

    // Заспавненный визуальный эффект. Что с ним делать — решает конкретный бафф.
    protected GameObject SpawnedEffect { get; private set; }

    private float _buffDuration;
    protected float TotalDuration { get; private set; }

    // Публичный доступ нужен BuffTrackerService для сохранения снимка.
    public float RemainingDuration => _buffDuration;

    private readonly ICoroutineRunner _runner;
    protected readonly ITimeService Time;
    private readonly IAssetLoader _assetLoader;
    private readonly BuffStaticData _buffStaticData;

    public BuffBase(
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      BuffStaticData buffStaticData,
      GameObject buffOwner
      )
    {
      if (buffStaticData.Class == BuffClassName.None)
        throw new InvalidOperationException(
          "[BuffBase] BuffStaticData.Class = None — это недопустимо.");

      if (buffStaticData.Class == BuffClassName.BuffBase)
        throw new InvalidOperationException(
          "[BuffBase] Нельзя создавать экземпляр BuffBase напрямую. " +
          "BuffBase — абстрактный базовый тип, используй конкретный подкласс.");

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

    // ─── Переопределяемые методы ─────────────────────────────────────────

    protected virtual void BurstActivation() { }
    protected virtual void ConstantActivation() { }

    // Вызывается один раз при старте Duration-баффа, до первого тика.
    protected virtual void OnDurationStarted() { }

    // Вызывается каждый кадр пока бафф активен.
    protected virtual void OnDurationTick() { }

    // Вызывается один раз когда Duration-бафф заканчивается.
    protected virtual void OnDurationEnded() { }

    // Вызывается при восстановлении Constant-баффа из сохранения.
    // Стат-эффект уже записан в PlayerStats, переопределяй только для визуала.
    protected virtual void OnConstantRestored() { }

    // ─── Публичный API ───────────────────────────────────────────────────

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
    /// Устанавливает оставшееся время действия перед вызовом Activate().
    /// Нужен для восстановления Duration-баффов из сохранения с точным остатком.
    /// </summary>
    public void SetRemainingDuration(float remainingDuration)
    {
      _buffDuration = Mathf.Max(0f, remainingDuration);
    }

    /// <summary>
    /// Восстанавливает Constant-бафф из сохранения.
    /// Помечает бафф как активный, но НЕ применяет стат-эффекты повторно —
    /// они уже лежат в сохранённом PlayerStats.
    /// Вызывает OnConstantRestored() для подклассов, которым нужно восстановить визуал.
    /// </summary>
    public void RestoreConstantBuff()
    {
      _buffStateRP.Value = BuffState.Active;
      OnConstantRestored();
    }

    // ─── Управление визуальным эффектом ──────────────────────────────────

    /// <summary>
    /// Инстанциирует префаб визуального эффекта через Addressables.
    /// Результат сохраняется в SpawnedEffect.
    /// </summary>
    protected async UniTask SpawnEffectAsync(Transform parent = null)
    {
      if (_buffStaticData.BuffEffectPrefab == null ||
          string.IsNullOrEmpty(_buffStaticData.BuffEffectPrefab.AssetGUID))
        return;

      SpawnedEffect = await _assetLoader.InstantiateAsync(
        _buffStaticData.BuffEffectPrefab,
        parent
        );
    }

    /// <summary>
    /// Уничтожает заспавненный визуальный эффект.
    /// </summary>
    protected void DestroyEffect()
    {
      if (SpawnedEffect != null)
        GameObject.Destroy(SpawnedEffect);

      SpawnedEffect = null;
    }

    // ─── Приватная механика активации ────────────────────────────────────

    private void RunBurstActivation()
    {
      _buffStateRP.Value = BuffState.Active;
      BurstActivation();
      _buffStateRP.Value = BuffState.Disabled;
    }

    private void RunConstantActivation()
    {
      _buffStateRP.Value = BuffState.Active;
      ConstantActivation();
    }

    private IEnumerator RunDurationActivation()
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
    }
  }
}
