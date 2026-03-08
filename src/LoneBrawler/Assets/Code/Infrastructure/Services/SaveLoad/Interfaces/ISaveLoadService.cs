// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;

namespace Code.Infrastructure.Services.SaveLoad.Interfaces
{
  public interface ISaveLoadService
  {
    /// <summary>
    /// Write to static PlayerProgress class and then serialize it.
    /// </summary>
    public void SaveProgress(bool isInitial = false, bool skipUTC = false);

    /// <summary>
    /// Load serialized progress
    /// </summary>
    /// <returns>PlayerProgress</returns>
    public GameProgress LoadProgress();
    SystemSettings LoadSettings();
  }
}
