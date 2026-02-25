// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

namespace Code.Infrastructure.Services.BuffService.Interfaces
{
  public interface IBuffTrackerService : IProgressWriter, IProgressReader
  {
    /// <summary>
    /// Регистрирует бафф в трекере. Вызывается после создания баффа через IBuffFactory.
    /// Activate() должен быть вызван отдельно.
    /// </summary>
    void AddBuff(BuffBase buff, BuffClassName className);

    /// <summary>
    /// Возвращает все активные баффы игрока по классу.
    /// </summary>
    IReadOnlyList<BuffBase> GetPlayerBuffs(BuffClassName className);
  }
}
