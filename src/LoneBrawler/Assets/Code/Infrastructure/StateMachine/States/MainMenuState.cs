// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.UI.Elements.Common.LoadingScreen.Interfaces;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  internal class MainMenuState : IGameState
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;

    private ISceneLoader _sceneLoader;
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IStaticDataService _staticDataService;
    private readonly IUIFactory _uiFactory;
    private readonly IGameFactory _gameFactory;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IAssetLoader _assetLoader;

    public MainMenuState(
      GameStateMachine gameStateMachine,
      ILoadScreen curtain)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _sceneLoader = RootContext.Resolve<ISceneLoader>();

      _persistentProgressService = RootContext.Resolve<IPersistentProgressService>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();

      _uiFactory = RootContext.Resolve<IUIFactory>();
      _gameFactory = RootContext.Resolve<IGameFactory>();

      _saveLoadService = RootContext.Resolve<ISaveLoadService>();

      _assetLoader = RootContext.Resolve<IAssetLoader>();

      _gameStateMachine = gameStateMachine;

      _curtain = curtain;
    }

    public async void Enter()
    {
      try
      {
        _logger.Log("Entered state");

        _curtain.Show();

        _assetLoader.Cleanup();
        _uiFactory.Cleanup();
        _gameFactory.Cleanup();

        await _uiFactory.WarmUp();
        _logger.Log("UIFactory WarmUp done");

        await _sceneLoader.LoadPlatformBased(
          SceneAddresses.MainMenuAddress,
          _staticDataService.BuildConfig.TargetPlatform,
          onSceneLoaded: OnLevelLoadedAsync);

        _logger.Log("LoadAsync returned");
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"Entering state failed: {exception}");
        throw exception;
      }
    }

    public void Exit()
    {
      _logger.Log("Exited state");

      _curtain.Hide();
    }

    private async void OnLevelLoadedAsync()
    {
      _logger.Log("Loading content for the active level...");

      try
      {
        InitUIRoot();
        await InitMainMenuAsync();

        _gameStateMachine.EnterState<GameLoopState>();
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadLevel failed: {exception}");
      }
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    private async UniTask InitMainMenuAsync() =>
      await _uiFactory.CreateMainMenuAsync();
  }
}
