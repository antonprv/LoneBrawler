// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Infrastructure.Serialization;
using Code.Data.SaveData;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerPrefs.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.Time;
using Code.UI.Services.InventoryService.Interfaces;

namespace Code.Infrastructure.Services.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";
    private const string SystemSettingsKey = "System";

    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameFactory _gameFactory;
    private readonly ITimeService _timeService;
    private readonly IPlayerPrefsService _playerPrefs;
    private readonly IBuffTrackerService _buffTracker;
    private readonly IInventoryService _inventoryService;
    private readonly ISoundService _soundService;

    public SaveLoadService(
      IPersistentProgressService progressService,
      IGameFactory gameFactory,
      ITimeService timeService,
      IPlayerPrefsService playerPrefsService,
      IBuffTrackerService buffTracker,
      IInventoryService inventoryService,
      ISoundService soundService
      )
    {
      _persistentProgressService = progressService;
      _gameFactory = gameFactory;
      _timeService = timeService;
      _playerPrefs = playerPrefsService;
      _buffTracker = buffTracker;
      _inventoryService = inventoryService;
      _soundService = soundService;
    }

    public void SaveProgress(bool isInitial = false, bool skipUTC = false)
    {
      foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
      {
        if (progressWriter != null)
          progressWriter.WriteToProgress(_persistentProgressService.Progress);
      }

      if (skipUTC == false)
      {
        _persistentProgressService.Progress.SaveTimeUTC =
          isInitial ? 0 : _timeService.UtcNow.Ticks;
      }

      if (_persistentProgressService.Progress != null)
        _buffTracker.WriteToProgress(_persistentProgressService.Progress);

      if (_persistentProgressService.SystemSettings != null)
        _soundService.WriteToSettings(_persistentProgressService.SystemSettings);

      _persistentProgressService.Progress.Inventory = _inventoryService.GetSaveData();

      _playerPrefs
        .SetString(
        SystemSettingsKey,
        _persistentProgressService.SystemSettings.ToSerialized()
        );

      _playerPrefs
        .SetString(
        ProgressKey,
        _persistentProgressService.Progress.ToSerialized()
        );

      _playerPrefs.Save();
    }

    public GameProgress LoadProgress() =>
      _playerPrefs.GetString(ProgressKey)?.ToDeserialized<GameProgress>();

    public SystemSettings LoadSettings() =>
      _playerPrefs.GetString(SystemSettingsKey)?.ToDeserialized<SystemSettings>();
  }
}
