// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Data.SaveData.Buffs;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Services.BuffService
{
  public class BuffTrackerService : IBuffTrackerService
  {
    // Single list of all player buffs.
    // Key — BuffClass enum, value — list of instances (a buff can be applied several times).
    private readonly Dictionary<BuffClassName, List<BuffBase>> _playerBuffs = new();

    private readonly IBuffFactory _buffFactory;
    private readonly IPlayerReader _playerReader;
    private readonly IGameLog _logger;

    public BuffTrackerService(
      IBuffFactory buffFactory,
      IPlayerReader playerReader,
      IGameLog gameLog
      )
    {
      _buffFactory = buffFactory;
      _playerReader = playerReader;
      _logger = gameLog;
    }

    #region IBuffTrackerService

    public void AddBuff(BuffBase buff, BuffClassName className)
    {
      if (!_playerBuffs.TryGetValue(className, out var list))
      {
        list = new List<BuffBase>();
        _playerBuffs[className] = list;
      }

      list.Add(buff);
    }

    public void RemoveBuff(BuffBase buff, BuffClassName className)
    {
      if (!_playerBuffs.TryGetValue(className, out var list))
      {
        list = new List<BuffBase>();
        _playerBuffs[className] = list;
      }

      list.Remove(buff);
    }

    public IReadOnlyList<BuffBase> GetPlayerBuffs(BuffClassName className)
    {
      if (_playerBuffs.TryGetValue(className, out var list))
        return list;

      return System.Array.Empty<BuffBase>();
    }

    #endregion

    #region IProgressWriter

    /// <summary>
    /// Saves snapshots of all current buffs to GameProgress.
    /// Burst-buffs in Disabled state are not saved — they've already finished their work.
    /// </summary>
    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.BuffsRegistry.PlayerBuffs.Clear();

      foreach (var (className, buffs) in _playerBuffs)
      {
        foreach (var buff in buffs)
        {
          BuffState state = buff.BuffStateRP.CurrentValue;

          if (state == BuffState.Disabled) continue;

          playerProgress.BuffsRegistry.PlayerBuffs.Add(new BuffSaveEntry
          {
            ClassName = className,
            ActivationType = buff.ActivationType,
            State = state,
            RemainingDuration = buff.RemainingDuration,
          });
        }
      }
    }

    #endregion

    #region IProgressReader

    /// <summary>
    /// Restores all buffs from save.
    /// Called after the player has been created and is available through IPlayerReader.
    /// </summary>
    public void ReadProgress(GameProgress playerProgress)
    {
      if (playerProgress.BuffsRegistry?.PlayerBuffs == null) return;
      if (playerProgress.BuffsRegistry.PlayerBuffs.Count == 0) return;

      RestoreBuffsAsync(playerProgress).Forget();
    }

    #endregion

    #region Private API

    private async UniTaskVoid RestoreBuffsAsync(GameProgress playerProgress)
    {
      GameObject player = _playerReader.GetPlayer();

      if (player == null)
      {
        _logger.Log(LogType.Error,
          "[BuffTrackerService] No player available when restoring buffs. " +
          "InformProgressReaders() should be called after player creation.");
        return;
      }

      foreach (BuffSaveEntry entry in playerProgress.BuffsRegistry.PlayerBuffs)
      {
        await RestoreSingleBuffAsync(entry, player);
      }
    }

    private async UniTask RestoreSingleBuffAsync(BuffSaveEntry entry, GameObject player)
    {
      BuffBase buff = await _buffFactory.CreateBuff(entry.ClassName, player);
      if (buff == null) return;

      AddBuff(buff, entry.ClassName);

      switch (entry.ActivationType)
      {
        case BuffActivationType.Duration:
          RestoreDurationBuff(buff, entry);
          break;

        case BuffActivationType.Constant:
          // Stat-effects are already in PlayerStats. Just mark as active and restore visuals.
          buff.RestoreConstantBuff();
          break;

        // Burst in Active state is theoretically unreachable (Burst immediately transitions to Disabled),
        // but just in case — simply skip.
        case BuffActivationType.Burst:
        case BuffActivationType.None:
        default:
          break;
      }
    }

    private static void RestoreDurationBuff(BuffBase buff, BuffSaveEntry entry)
    {
      // Set precise remaining time, then activate.
      // Activate() will pick up the modified _buffDuration.
      buff.SetRemainingDuration(entry.RemainingDuration);
      buff.Activate();
    }

    #endregion

  }
}
