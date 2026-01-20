// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;

namespace Code.Infrastructure.Services.PersistentProgress
{
  public interface ISavedProgressWriter
  {
    public void UpdateProgress(PlayerProgress playerProgress);
  }

  public interface ISavedProgressReader
  {
    public void LoadProgress(PlayerProgress playerProgress);
  }
}
