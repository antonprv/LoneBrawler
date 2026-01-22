// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;

namespace Code.Infrastructure.Services.SaveLoad
{
  public interface ISaveLoadService
  {
    /// <summary>
    /// Write to static PlayerProgress class and then serialize it.
    /// </summary>
    public void SaveProgress();

    /// <summary>
    /// Load serialized progress
    /// </summary>
    /// <returns>PlayerProgress</returns>
    public PlayerProgress LoadProgress();
  }
}
