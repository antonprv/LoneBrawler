// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Gameplay.Common.Time
{
  public interface ITimeService
  {
    float DeltaTime { get; }

    float UnscaledDeltaTime { get; }

    DateTime UtcNow { get; }

    void StopTime();
    void StartTime();
  }
}
