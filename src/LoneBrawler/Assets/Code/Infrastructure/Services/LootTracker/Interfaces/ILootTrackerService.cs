// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Infrastructure.Services.LootTracker.Interfaces
{
  public interface ILootTrackerService
  {
    event Action OnValueChanged;

    public int Souls { get; set; }
  }
}
