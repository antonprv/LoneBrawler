// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;

namespace Code.Infrastructure.Services.PersistentProgress.Interfaces
{
  public interface IProgressWatcher
  {
  }

  public interface IProgressReader : IProgressWatcher
  {
    void ReadProgress(GameProgress playerProgress);
  }

  public interface IProgressWriter : IProgressWatcher
  {
    public void WriteToProgress(GameProgress playerProgress);
  }
}
