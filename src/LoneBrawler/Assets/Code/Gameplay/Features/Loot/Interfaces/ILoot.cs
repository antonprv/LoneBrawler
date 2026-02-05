// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILoot
  {
    int Souls { get; set; }

    event Action OnCollected;
  }
}
