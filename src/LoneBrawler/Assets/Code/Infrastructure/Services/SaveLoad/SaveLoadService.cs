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
    private readonly IPersistentProgressService _progressService;
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
      _progressService = progressService;
      _gameFactory = gameFactory;
      _timeService = timeService;
      _playerPrefs = playerPrefsService;
      _buffTracker = buffTracker;
      _inventoryService = inventoryService;
      _soundService = soundService;
    }

    public void SaveProgress(bool isInitial = false, bool skipUTC = false)
    {
      // When starting a new game, skip writing gameplay state into the fresh progress.
      // Buffs, inventory and factory writers all belong to the previous session and
      // must not bleed into the newly created GameProgress.
      if (!isInitial)
      {
        foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
        {
          if (progressWriter != null)
            progressWriter.WriteToProgress(_progressService.Progress);
        }

        if (_progressService.Progress != null)
          _buffTracker.WriteToProgress(_progressService.Progress);

        _progressService.Progress.Inventory = _inventoryService.GetSaveData();
      }

      if (skipUTC == false)
      {
        _progressService.Progress.SaveTimeUTC =
          isInitial ? 0 : _timeService.UtcNow.Ticks;
      }

      // Sound settings are session-independent — always persist them.
      if (_progressService.SystemSettings != null)
        _soundService.WriteToSettings(_progressService.SystemSettings);

      _playerPrefs
        .SetString(
        _progressService.SystemSettingsKey,
        _progressService.SystemSettings.ToSerialized()
        );

      _playerPrefs
        .SetString(
        _progressService.ProgressKey,
        _progressService.Progress.ToSerialized()
        );

      _playerPrefs.Save();
    }

    public GameProgress LoadProgress() =>
      _playerPrefs.GetString(_progressService.ProgressKey)?.ToDeserialized<GameProgress>();

    public SystemSettings LoadSettings() =>
      _playerPrefs.GetString(_progressService.SystemSettingsKey)?.ToDeserialized<SystemSettings>();
  }
}
