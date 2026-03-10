// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.InventoryService.Interfaces;

using Code.Common.CustomTypes.Infrastructure.Serialization;
using Code.Data.SaveData;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using UnityEngine;

using Code.Infrastructure.Services.SoundService.Interfaces;

namespace Code.Infrastructure.Services.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";
    private const string SystemSettingsKey = "System";

    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameFactory _gameFactory;
    private readonly ITimeService _timeService;
    private readonly IBuildConfigSubservice _buildConfig;
    private readonly IBuffTrackerService _buffTracker;
    private readonly IInventoryService _inventoryService;
    private readonly ISoundService _soundService;

    public SaveLoadService(
      IPersistentProgressService progressService,
      IGameFactory gameFactory,
      ITimeService timeService,
      IBuildConfigSubservice buildConfig,
      IBuffTrackerService buffTracker,
      IInventoryService inventoryService,
      ISoundService soundService
      )
    {
      _persistentProgressService = progressService;
      _gameFactory = gameFactory;
      _timeService = timeService;
      _buildConfig = buildConfig;
      _buffTracker = buffTracker;
      _inventoryService = inventoryService;
      _soundService = soundService;
    }

    public void SaveProgress(bool isInitial = false, bool skipUTC = false)
    {
      foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
        progressWriter.WriteToProgress(_persistentProgressService.Progress);

      if (skipUTC == false)
      {
        _persistentProgressService.Progress.SaveTimeUTC =
          isInitial ? 0 : _timeService.UtcNow.Ticks;
      }

      _buffTracker.WriteToProgress(_persistentProgressService.Progress);
      _soundService.WriteToSettings(_persistentProgressService.SystemSettings);

      _persistentProgressService.Progress.Inventory = _inventoryService.GetSaveData();

      if (_buildConfig.UseCloudSave == true)
      {
        RedefineYG
          .PlayerPrefs
          .SetString(SystemSettingsKey, _persistentProgressService.SystemSettings.ToSerialized());

        RedefineYG.
          PlayerPrefs
          .SetString(ProgressKey, _persistentProgressService.Progress.ToSerialized());

        RedefineYG.PlayerPrefs.Save();
      }
      else
      {
        PlayerPrefs
          .SetString(SystemSettingsKey, _persistentProgressService.SystemSettings.ToSerialized());

        PlayerPrefs
          .SetString(ProgressKey, _persistentProgressService.Progress.ToSerialized());

        PlayerPrefs.Save();
      }
    }

    public GameProgress LoadProgress()
    {
      if (_buildConfig.UseCloudSave == true)
        return RedefineYG.PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<GameProgress>();

      return PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<GameProgress>();
    }

    public SystemSettings LoadSettings()
    {
      if (_buildConfig.UseCloudSave == true)
        return RedefineYG.PlayerPrefs.GetString(SystemSettingsKey)?.ToDeserialized<SystemSettings>();

      return PlayerPrefs.GetString(SystemSettingsKey)?.ToDeserialized<SystemSettings>();
    }
  }
}
