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
    /// Registers a buff in the tracker. Called after creating a buff through IBuffFactory.
    /// Activate() must be called separately.
    /// </summary>
    void AddBuff(BuffBase buff, BuffClassName className);

    /// <summary>
    /// Returns all active player buffs by class.
    /// </summary>
    IReadOnlyList<BuffBase> GetPlayerBuffs(BuffClassName className);
    void RemoveBuff(BuffBase buff, BuffClassName className);
  }
}
