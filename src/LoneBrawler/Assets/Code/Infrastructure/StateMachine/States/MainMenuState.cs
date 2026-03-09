// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Configs;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.StateMachine.States
{
  internal class MainMenuState : IGameState
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;
    private readonly IMusicPlayerHolder _musicPlayerHolder;
    private readonly ISceneLoader _sceneLoader;
    private readonly IStaticDataService _staticData;
    private readonly IUIFactory _uiFactory;
    private readonly IGameFactory _gameFactory;
    private readonly IAssetLoader _assetLoader;
    private string _mainMenuSceneName;

    /// <summary>
    /// Dedicated main menu state.
    /// </summary>
    public MainMenuState(
      GameStateMachine gameStateMachine,
      ILoadScreen curtain,
      IGameLog gameLog,
      ISceneLoader sceneLoader,
      IStaticDataService staticDataService,
      IUIFactory uIFactory,
      IGameFactory gameFactory,
      IAssetLoader assetLoader,
      IMusicPlayerHolder musicPlayerHolder
      )
    {
      _logger = gameLog;
      _sceneLoader = sceneLoader;

      _staticData = staticDataService;

      _uiFactory = uIFactory;
      _gameFactory = gameFactory;


      _assetLoader = assetLoader;

      _gameStateMachine = gameStateMachine;

      _curtain = curtain;

      _musicPlayerHolder = musicPlayerHolder;
      }

    public async void Enter()
    {
      _mainMenuSceneName = SceneAddresses.MainMenuAddress;

      try
      {
        StopLevelMusic();

        _logger.Log("Entered state");

        _curtain.Show();

        _assetLoader.Cleanup();
        _uiFactory.Cleanup();
        _gameFactory.Cleanup();

        await _uiFactory.WarmUp();
        _logger.Log("UIFactory WarmUp done");

        await _sceneLoader.LoadPlatformBased(
          _mainMenuSceneName,
          _staticData.BuildConfig.TargetPlatform,
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
        await LoadLevelMusicAsync();

        InitUIRoot();
        await InitMainMenuAsync();

        PlayLevelMusic();

        _gameStateMachine.EnterState<GameLoopState>();
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadLevel failed: {exception}");
      }
    }

    private void StopLevelMusic()
    {
      if (_musicPlayerHolder.Current != null)
        _musicPlayerHolder.Current.Stop();
    }

    private void PlayLevelMusic() => _musicPlayerHolder.Current.Play();

    private async UniTask LoadLevelMusicAsync()
    {
      MusicPlaylist playlist = await _staticData.LevelMusic.ForLevelAsync(_mainMenuSceneName);
      MusicPlayerConfig playerConfig = _staticData.MusicConfig.Confg;
      _musicPlayerHolder.Current.SetConfig(playerConfig);
      _musicPlayerHolder.Current.SetPlaylist(playlist);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    private async UniTask InitMainMenuAsync() =>
      await _uiFactory.CreateMainMenuAsync();
  }
}
