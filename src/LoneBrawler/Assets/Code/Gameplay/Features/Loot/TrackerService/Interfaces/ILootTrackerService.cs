// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Features.Loot.TrackerService.Interfaces
{
  public interface ILootTrackerService
  {
    event Action OnValueChanged;

    public int Souls { get; set; }
  }
}
