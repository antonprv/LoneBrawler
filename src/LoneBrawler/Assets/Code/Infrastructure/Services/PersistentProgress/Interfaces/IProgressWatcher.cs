// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.PersistentProgress.Interfaces
{
  public interface IProgressWatcher
  {
  }

  public interface IProgressReader : IProgressWatcher
  {
    public void ReadProgress(GameProgress playerProgress);
  }

  public interface IProgressReaderAsync : IProgressWatcher
  {
    public UniTask ReadProgressAsync(GameProgress playerProgress);
  }

  public interface IProgressWriter : IProgressWatcher
  {
    public void WriteToProgress(GameProgress playerProgress);
  }

  public interface ISettingsReader : IProgressWatcher
  {
    void ReadSettings(SystemSettings systemSettings);
  }

  public interface ISettingsWriter : IProgressWatcher
  {
    void WriteToSettings(SystemSettings systemSettings);
  }
}
