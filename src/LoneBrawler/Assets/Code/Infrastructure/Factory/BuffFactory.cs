// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Common.Extensions.Async;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;
using Code.Gameplay.Features.Buffs.Duration;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Factory
{
  public class BuffFactory : IBuffFactory
  {
    private readonly IBuffDataSubservice _buffData;
    private readonly ICoroutineRunner _runner;
    private readonly ITimeService _time;
    private readonly IAssetLoader _assetLoader;

    private Dictionary<BuffClassName, Func<BuffStaticData, GameObject, BuffBase>> _constructors;

    public BuffFactory(
      IBuffDataSubservice buffData,
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader
      )
    {
      _buffData = buffData;
      _runner = coroutineRunner;
      _time = timeService;
      _assetLoader = assetLoader;

      FillConstructors();
    }

    // ─── Публичный API ───────────────────────────────────────────────────

    /// <summary>
    /// Создаёт экземпляр баффа и возвращает его.
    /// Регистрация в IBuffTrackerService и вызов Activate() — на вызывающей стороне.
    /// </summary>
    public async UniTask<BuffBase> CreateBuff(BuffClassName buffClass, GameObject buffOwner)
    {
      if (buffClass == BuffClassName.None)
        throw new ArgumentException(
          "[BuffFactory] Попытка создать бафф с типом None — это недопустимо.");

      if (buffClass == BuffClassName.BuffBase)
        throw new ArgumentException(
          "[BuffFactory] BuffBase — абстрактный базовый тип, нельзя создавать напрямую.");

      return await InstantiateBuff(buffClass, buffOwner);
    }

    // ─── Приватный API ───────────────────────────────────────────────────

    private void FillConstructors()
    {
      _constructors = new()
      {
        {
          BuffClassName.DamageBuff,
          (data, owner) => new DamageBuff(_runner, _time, _assetLoader, data, owner)
        },
        {
          BuffClassName.RageBuff,
          (data, owner) => new RageBuff(_runner, _time, _assetLoader, data, owner)
        },
        {
          BuffClassName.GodBuff,
          (data, owner) => new GodBuff(_runner, _time, _assetLoader, data, owner)
        },
        {
          BuffClassName.SpeedBuff,
          (data, owner) => new SpeedBuff(_runner, _time, _assetLoader, data, owner)
        },
        {
          BuffClassName.RegenBuff,
          (data, owner) => new RegenBuff(_runner, _time, _assetLoader, data, owner)
        },
        {
          BuffClassName.HealthBuff,
          (data, owner) => new HealthBuff(_runner, _time, _assetLoader, data, owner)
        },
        {
          BuffClassName.HealthPotionBuff,
          (data, owner) => new HealthPotionBuff(_runner, _time, _assetLoader, data, owner)
        },
      };
    }

    private async UniTask<BuffBase> InstantiateBuff(BuffClassName buffClass, GameObject buffOwner)
    {
      if (!_constructors.TryGetValue(buffClass, out var constructor))
      {
        Debug.LogError($"[BuffFactory] Нет конструктора для баффа '{buffClass}'. " +
                       $"Зарегистрируй его в FillConstructors().");
        return null;
      }

      BuffStaticData buffData = await _buffData.ForBuffAsync(buffClass);

      if (buffData == null)
      {
        Debug.LogError($"[BuffFactory] BuffStaticData для '{buffClass}' не найдена в манифесте.");
        return null;
      }

      return constructor(buffData, buffOwner);
    }
  }
}
