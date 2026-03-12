// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Threading;

using Code.Infrastructure.StateMachine.Types;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Configs;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.AssetsPreloader.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.StateMachine.States
{
  public class MainMenuState : IGameState
  {
    #region StateType

    public StateType Type => StateType.MainMenu;

    #endregion

    private readonly IGameLog _logger;

    private readonly IGameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;
    private readonly IMusicPlayerHolder _musicPlayerHolder;
    private readonly ISceneLoader _sceneLoader;
    private readonly IStaticDataService _staticData;
    private readonly IUIFactory _uiFactory;
    private readonly IGameFactory _gameFactory;
    private readonly IAssetLoader _assetLoader;
    private readonly IAssetsPreloader _assetsPreloader;

    private string _mainMenuSceneName;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Dedicated main menu state.
    /// The curtain is only hidden after the menu music is fully loaded,
    /// so the player always enters to audio — never to silence.
    /// </summary>
    public MainMenuState(
      IGameStateMachine gameStateMachine,
      ILoadScreen curtain,
      IGameLog gameLog,
      ISceneLoader sceneLoader,
      IStaticDataService staticDataService,
      IUIFactory uIFactory,
      IGameFactory gameFactory,
      IAssetLoader assetLoader,
      IMusicPlayerHolder musicPlayerHolder,
      IAssetsPreloader assetsPreloader
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
      _assetsPreloader = assetsPreloader;
    }

    public void Enter() => EnterAsync().Forget();

    private async UniTask EnterAsync()
    {
      _mainMenuSceneName = SceneAddresses.MainMenuAddress;

      _cts = new CancellationTokenSource();

      try
      {
        StopLevelMusic();

        _logger.Log("Entered state");

        _curtain.Show();

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
      _cts?.Cancel();
      _cts?.Dispose();
      _cts = null;
    }

    private async void OnLevelLoadedAsync()
    {
      _logger.Log("Loading content for the active level...");

      var ct = _cts?.Token ?? CancellationToken.None;

      try
      {
        // 1. Assign playlist & config to the music player.
        await LoadLevelMusicAsync();

        if (ct.IsCancellationRequested) return;

        // 2. Preload all audio clips before showing the menu.
        //    The curtain stays visible until every track is in memory,
        //    so the player never enters the Main Menu to silence.
        _logger.Log("Preloading menu music...");
        await _assetsPreloader.PreloadMusicAsync(_mainMenuSceneName, ct);
        _logger.Log("Menu music preloaded");

        if (ct.IsCancellationRequested) return;

        // 3. Build the UI, start music, then reveal the screen.
        InitUIRoot();
        await InitMainMenuAsync();

        PlayLevelMusicAsync().Forget();

        _curtain.Hide();
      }
      catch (OperationCanceledException)
      {
        _logger.Log("MainMenuState.OnLevelLoaded cancelled");
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadLevel failed: {exception}");
      }
    }

    private void StopLevelMusic()
    {
      if (_musicPlayerHolder.Current != null)
        _musicPlayerHolder.Current.Stop().Forget();
    }

    private async UniTaskVoid PlayLevelMusicAsync()
    {
      if (_musicPlayerHolder.Current != null)
        await _musicPlayerHolder.Current.Play();
    }

    private async UniTask LoadLevelMusicAsync()
    {
      MusicPlaylist playlist = await _staticData.LevelMusic.ForLevelAsync(_mainMenuSceneName);
      MusicPlayerConfig playerConfig = _staticData.MusicConfig.Confg;

      if (_musicPlayerHolder.Current == null)
      {
        _logger.Log(LogType.Error, "MusicPlayer is not registered yet!");
        return;
      }

      _musicPlayerHolder.Current.SetConfig(playerConfig);
      _musicPlayerHolder.Current.SetPlaylist(playlist);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    private async UniTask InitMainMenuAsync() =>
      await _uiFactory.CreateMainMenuAsync();
  }
}
