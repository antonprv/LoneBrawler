// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadProgressState : IGameState
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly IPersistentProgressService _progressService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IStaticDataService _staticData;
    private readonly IAssetLoader _assetLoader;
    private readonly IGameFactory _gameFactory;
    private readonly IUIFactory _uiFactory;
    private readonly ISoundService _soundService;

    /// <summary>
    /// Loads player progress and settings data
    /// </summary>
    public LoadProgressState(
      GameStateMachine gameStateMachine,
      IGameLog gameLog,
      IPersistentProgressService persistentProgress,
      ISaveLoadService saveLoadService,
      IStaticDataService staticDataService,
      IAssetLoader assetLoader,
      IGameFactory gameFactory,
      IUIFactory uIFactory,
      ISoundService soundService
      )
    {
      _logger = gameLog;
      _progressService = persistentProgress;
      _saveLoadService = saveLoadService;
      _staticData = staticDataService;
      _assetLoader = assetLoader;
      _gameFactory = gameFactory;
      _uiFactory = uIFactory;
      _soundService = soundService;

      _gameStateMachine = gameStateMachine;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      InitNewProgressIfNull();
      InitSettingsIfNull();
      InitializeSoundService();

      _logger.Log($"Transitioning to state {nameof(LoadLevelState)}");
      _gameStateMachine.EnterState<MainMenuState>();
    }

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
