// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

namespace Code.Infrastructure.Services.SaveLoad.Interfaces
{
  public interface ILiveProgressSync
  {
    float SyncIntervalSeconds { get; }

    void StartSyncLoop();
    void StopSyncLoop();
  }
}
