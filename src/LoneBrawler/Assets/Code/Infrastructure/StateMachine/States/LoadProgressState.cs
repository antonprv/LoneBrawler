// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Data.SaveData.Types;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.Infrastructure.StateMachine.Types;
using Code.UI.Services.PlatformControls.Interfaces;

using UApp = UnityEngine.Application;
using UPlatform = UnityEngine.RuntimePlatform;

namespace Code.Infrastructure.StateMachine.States
{
  public class LoadProgressState : IGameState
  {
    #region StateType

    public StateType Type => StateType.LoadProgress;

    #endregion

    private readonly IGameLog _logger;

    private readonly IGameStateMachine _gameStateMachine;
    private readonly IPlatformControls _platformControls;
    private readonly IPersistentProgressService _progressService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IStaticDataService _staticData;
    private readonly ISoundService _soundService;
    private readonly IBuffTrackerService _buffTracker;

    /// <summary>
    /// Loads player progress and settings data
    /// </summary>
    public LoadProgressState(
      IGameStateMachine gameStateMachine,
      IGameLog gameLog,
      IPersistentProgressService persistentProgress,
      ISaveLoadService saveLoadService,
      IStaticDataService staticDataService,
      ISoundService soundService,
      IBuffTrackerService buffTracker,
      IPlatformControls platformControls
      )
    {
      _logger = gameLog;
      _progressService = persistentProgress;
      _saveLoadService = saveLoadService;
      _staticData = staticDataService;
      _soundService = soundService;

      _buffTracker = buffTracker;

      _gameStateMachine = gameStateMachine;

      _platformControls = platformControls;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      ClenupSession();

      InitNewProgressIfNull();
      InitSettingsIfNull();
      SetPlatformControls();
      InitializeSoundService();

      _logger.Log($"Transitioning to state {nameof(LoadLevelState)}");
      _gameStateMachine.EnterState<MainMenuState>();
    }

    private void SetPlatformControls()
    {
      if (UApp.platform == UPlatform.Android)
        _platformControls.SetScheme(ControlScheme.Mobile);
    }

    private void ClenupSession() => _buffTracker.Cleanup();

    public void Exit() => _logger.Log("Exited state");

    private void InitNewProgressIfNull()
    {
      _logger.Log("Loading player progress...");

      GameProgress loadedProgress = _saveLoadService.LoadProgress();

      if (loadedProgress == null)
      {
        _progressService.Progress = NewProgress();
        _saveLoadService.SaveProgress(isInitial: true);
      }
      else
        _progressService.Progress = loadedProgress;
    }

    private void InitSettingsIfNull()
    {
      _logger.Log("Loading system settings...");

      SystemSettings loadedSettings = _saveLoadService.LoadSettings();

      if (loadedSettings == null)
      {
        _progressService.SystemSettings = NewSettings();
        _saveLoadService.SaveProgress(isInitial: true);
      }
      else
        _progressService.SystemSettings = loadedSettings;
    }

    private void InitializeSoundService() =>
      _soundService.ReadSettings(_progressService.SystemSettings);

    private SystemSettings NewSettings() => new();

    private GameProgress NewProgress() => new(
        _staticData.PlayerData,
        _staticData.InventoryConfig,
        SceneAddresses.MainSceneAddress);
  }
}
