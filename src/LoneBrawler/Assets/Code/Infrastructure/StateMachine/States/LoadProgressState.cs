// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Factory.Interfaces;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadProgress : IGameState
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly IPersistentProgressService _progressService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IStaticDataService _staticData;
    private readonly IAssetLoader _assetLoader;
    private readonly IGameFactory _gameFactory;
    private readonly IUIFactory _uiFactory;

    public LoadProgress(GameStateMachine gameStateMachine)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _progressService = RootContext.Resolve<IPersistentProgressService>();
      _saveLoadService = RootContext.Resolve<ISaveLoadService>();
      _staticData = RootContext.Resolve<IStaticDataService>();
      _assetLoader = RootContext.Resolve<IAssetLoader>();
      _gameFactory = RootContext.Resolve<IGameFactory>();
      _uiFactory = RootContext.Resolve<IUIFactory>();

      _gameStateMachine = gameStateMachine;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      InitNewProgressIfNull();

      _logger.Log($"Transitioning to state {nameof(LoadLevelState)}");
      _gameStateMachine.EnterState<MainMenuState>();
    }

    public void Exit() => _logger.Log("Exited state");

    private void InitNewProgressIfNull()
    {
      _logger.Log("Loading player progress...");

      var loadedProgress = _saveLoadService.LoadProgress();

      if (loadedProgress == null)
      {
        _progressService.Progress = NewProgress();
        _saveLoadService.SaveProgress(isInitial: true);
      }
      else
        _progressService.Progress = loadedProgress;
    }

    private GameProgress NewProgress()
    {
      Cleanup();
      return new(
        _staticData.PlayerData,
        _staticData.InventoryConfig,
        SceneAddresses.MainSceneAddress);
    }

    private void Cleanup()
    {
      _assetLoader.Cleanup();
      _gameFactory.Cleanup();
      _uiFactory.Cleanup();
    }
  }
}
