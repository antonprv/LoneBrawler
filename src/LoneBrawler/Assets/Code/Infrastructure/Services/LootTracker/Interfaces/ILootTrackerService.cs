// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using R3;

namespace Code.Infrastructure.Services.LootTracker.Interfaces
{
  public interface ILootTrackerService
  {
    public ReadOnlyReactiveProperty<int> SoulsRP { get; }
    public void AddSouls(int amount);
    public bool SpendSouls(int amount);
  }
}
