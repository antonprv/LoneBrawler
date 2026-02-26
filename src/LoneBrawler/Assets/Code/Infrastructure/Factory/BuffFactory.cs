// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;
using Code.Gameplay.Features.Buffs.Burst;
using Code.Gameplay.Features.Buffs.Constant;
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
    private readonly IGameLog _logger;

    private Dictionary<BuffClassName, Func<BuffStaticData, GameObject, BuffBase>> _constructors;

    public BuffFactory(
      IBuffDataSubservice buffData,
      ICoroutineRunner coroutineRunner,
      ITimeService timeService,
      IAssetLoader assetLoader,
      IGameLog gameLog
      )
    {
      _buffData = buffData;
      _runner = coroutineRunner;
      _time = timeService;
      _assetLoader = assetLoader;
      _logger = gameLog;

      FillConstructors();
    }

    #region Public API

    /// <summary>
    /// Creates a buff instance and returns it.
    /// Registration in IBuffTrackerService and calling Activate() are up to the caller.
    /// </summary>
    public async UniTask<BuffBase> CreateBuff(BuffClassName buffClass, GameObject buffOwner)
    {
      if (buffClass == BuffClassName.None)
        throw new ArgumentException(
          "[BuffFactory] Attempt to create a buff with type None — this is not allowed.");

      if (buffClass == BuffClassName.BuffBase)
        throw new ArgumentException(
          "[BuffFactory] BuffBase is an abstract base type, cannot be created directly.");

      return await InstantiateBuff(buffClass, buffOwner);
    }

    #endregion

    #region Private API

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
        _logger.Log(LogType.Error,
          $"[BuffFactory] No constructor found for buff '{buffClass}'. " +
                       $"Register it in FillConstructors().");
        return null;
      }

      BuffStaticData buffData = await _buffData.ForBuffAsync(buffClass);

      if (buffData == null)
      {
        _logger.Log(LogType.Error,
          $"[BuffFactory] BuffStaticData for '{buffClass}' was not found in manifest.");
        return null;
      }

      return constructor(buffData, buffOwner);
    }

    #endregion
  }
}
