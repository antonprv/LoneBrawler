// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Infrastructure.Serialization;
using Code.Data.SaveData;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using PlayerPrefs = RedefineYG.PlayerPrefs;

namespace Code.Infrastructure.Services.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";

    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameFactory _gameFactory;
    private readonly ITimeService _timeService;
    private readonly IBuildConfigSubservice _buildConfig;
    private readonly IBuffTrackerService _buffTracker;

    public SaveLoadService(
      IPersistentProgressService progressService,
      IGameFactory gameFactory,
      ITimeService timeService,
      IBuildConfigSubservice buildConfig,
      IBuffTrackerService buffTracker
      )
    {
      _persistentProgressService = progressService;
      _gameFactory = gameFactory;
      _timeService = timeService;
      _buildConfig = buildConfig;
      _buffTracker = buffTracker;
    }

    public void SaveProgress()
    {
      foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
        progressWriter.WriteToProgress(_persistentProgressService.Progress);

      _persistentProgressService.Progress.SaveTimeUTC = _timeService.UtcNow.Ticks;

      _buffTracker.WriteToProgress(_persistentProgressService.Progress);

      PlayerPrefs.SetString(ProgressKey, _persistentProgressService.Progress.ToSerialized());
    }

    public GameProgress LoadProgress()
    {
      return PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<GameProgress>();
    }
  }
}
