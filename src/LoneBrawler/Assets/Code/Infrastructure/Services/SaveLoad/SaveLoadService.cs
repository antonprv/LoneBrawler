// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Infrastructure.Serialization;
using Code.Data.SaveData;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.Time;

using UnityEngine;

namespace Code.Infrastructure.Services.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";

    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameFactory _gameFactory;
    private readonly ITimeService _timeService;

    public SaveLoadService(
      IPersistentProgressService progressService,
      IGameFactory gameFactory,
      ITimeService timeService
      )
    {
      _persistentProgressService = progressService;
      _gameFactory = gameFactory;
      _timeService = timeService;
    }

    public void SaveProgress()
    {
      foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
        progressWriter.WriteToProgress(_persistentProgressService.Progress);

      _persistentProgressService.Progress.SaveTimeUTC = _timeService.UtcNow.Ticks;

      PlayerPrefs.SetString(ProgressKey, _persistentProgressService.Progress.ToSerialized());
    }

    public GameProgress LoadProgress()
    {
      return PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<GameProgress>();
    }
  }
}
