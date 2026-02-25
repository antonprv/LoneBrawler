// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

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
    // Единый список всех баффов игрока.
    // Ключ — класс баффа, значение — список экземпляров (бафф может быть наложен несколько раз).
    private readonly Dictionary<BuffClassName, List<BuffBase>> _playerBuffs = new();

    private readonly IBuffFactory _buffFactory;
    private readonly IPlayerReader _playerReader;

    public BuffTrackerService(
      IBuffFactory buffFactory,
      IPlayerReader playerReader
      )
    {
      _buffFactory = buffFactory;
      _playerReader = playerReader;
    }

    // ─── IBuffTrackerService ─────────────────────────────────────────────

    public void AddBuff(BuffBase buff, BuffClassName className)
    {
      if (!_playerBuffs.TryGetValue(className, out var list))
      {
        list = new List<BuffBase>();
        _playerBuffs[className] = list;
      }

      list.Add(buff);
    }

    public IReadOnlyList<BuffBase> GetPlayerBuffs(BuffClassName className)
    {
      if (_playerBuffs.TryGetValue(className, out var list))
        return list;

      return System.Array.Empty<BuffBase>();
    }

    // ─── IProgressWriter ─────────────────────────────────────────────────

    /// <summary>
    /// Сохраняет снимки всех текущих баффов в GameProgress.
    /// Burst-баффы в состоянии Disabled не сохраняются — они уже отработали.
    /// </summary>
    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.BuffsRegistry.PlayerBuffs.Clear();

      foreach (var (className, buffs) in _playerBuffs)
      {
        foreach (var buff in buffs)
        {
          BuffState state = buff.BuffStateRP.CurrentValue;

          // Disabled-баффы восстанавливать нечего — они уже отработали.
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

    // ─── IProgressReader ─────────────────────────────────────────────────

    /// <summary>
    /// Восстанавливает все баффы из сохранения.
    /// Вызывается после того, как игрок создан и доступен через IPlayerReader.
    /// </summary>
    public void ReadProgress(GameProgress playerProgress)
    {
      if (playerProgress.BuffsRegistry?.PlayerBuffs == null) return;
      if (playerProgress.BuffsRegistry.PlayerBuffs.Count == 0) return;

      RestoreBuffsAsync(playerProgress).Forget();
    }

    // ─── Приватное восстановление ─────────────────────────────────────────

    private async UniTaskVoid RestoreBuffsAsync(GameProgress playerProgress)
    {
      GameObject player = _playerReader.GetPlayer();

      if (player == null)
      {
        Debug.LogError("[BuffTrackerService] Нет игрока при восстановлении баффов. " +
                       "InformProgressReaders() должен вызываться после создания игрока.");
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
          // Стат-эффекты уже в PlayerStats. Только помечаем активным и восстанавливаем визуал.
          buff.RestoreConstantBuff();
          break;

        // Burst в Active-состоянии теоретически недостижим (после Burst сразу Disabled),
        // но на всякий случай — просто пропускаем.
        case BuffActivationType.Burst:
        case BuffActivationType.None:
        default:
          break;
      }
    }

    private static void RestoreDurationBuff(BuffBase buff, BuffSaveEntry entry)
    {
      // Устанавливаем точный остаток времени, затем запускаем.
      // Activate() подхватит уже изменённый _buffDuration.
      buff.SetRemainingDuration(entry.RemainingDuration);
      buff.Activate();
    }
  }
}
