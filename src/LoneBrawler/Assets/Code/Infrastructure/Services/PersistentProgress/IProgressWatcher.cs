// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;

namespace Code.Infrastructure.Services.PersistentProgress
{
  public interface IProgressWatcher
  {
  }

  public interface IProgressReader : IProgressWatcher
  {
    public void ReadProgress(PlayerProgress playerProgress);
  }

  public interface IProgressWriter : IProgressWatcher
  {
    public void WriteToProgress(PlayerProgress playerProgress);
  }
}
