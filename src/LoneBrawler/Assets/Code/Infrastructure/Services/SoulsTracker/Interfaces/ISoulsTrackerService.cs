// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using R3;

namespace Code.Infrastructure.Services.SoulsTracker.Interfaces
{
  public interface ISoulsTrackerService : IProgressReader
  {
    public ReadOnlyReactiveProperty<int> SoulsRP { get; }
    public void AddSouls(int amount);
    public bool TrySpendSouls(int amount);
  }
}
