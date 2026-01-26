// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data;
using Code.Data.DataExtensions;
using Code.Infrastructure.Factory;
using Code.Infrastructure.Services.PersistentProgress;

using UnityEngine;

namespace Code.Infrastructure.Services.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";

    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameFactory _gameFactory;

    public SaveLoadService()
    {
      _persistentProgressService = RootContext.Resolve<IPersistentProgressService>();
      _gameFactory = RootContext.Resolve<IGameFactory>();
    }

    public void SaveProgress()
    {
      foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
        progressWriter.WriteToProgress(_persistentProgressService.Progress);

      PlayerPrefs.SetString(ProgressKey, _persistentProgressService.Progress.ToSerialized());
    }

    public PlayerProgress LoadProgress()
    {
      return PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<PlayerProgress>();
    }
  }
}
