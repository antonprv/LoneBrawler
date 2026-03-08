// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

#region Project-specifid imports

using Code.Common.CustomTypes.Infrastructure.Types;
using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Elements.Player;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.InventoryService.Interfaces;

#endregion

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

using UObject = UnityEngine.Object;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadLevelState : IGamePayloadedState<string>
  {
    #region State Infrastructure

    private readonly IGameLog _logger;
    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;

    #endregion

    #region State Data

    private GameObject _hud;
    private string _loadedSceneName;
    private LevelStaticData _levelData;

    #endregion

    #region Scene & Factories

    private readonly ISceneLoader _sceneLoader;
    private readonly IGameFactory _gameFactory;
    private readonly IUIFactory _uiFactory;
    private readonly ICameraManager _cameraManager;
    private readonly IAssetLoader _assetLoader;

    #endregion

    #region Services

    private readonly IStaticDataService _staticData;
    private readonly IPersistentProgressService _progressService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly ISoundService _soundService;
    private readonly IMusicPlayer _musicPlayer;

    #endregion

    #region Player

    private readonly IPlayerWriter _playerWriter;
    private readonly IPlayerReader _playerReader;

    #endregion

    #region Gameplay

    private readonly IBuffTrackerService _buffTracker;
    private readonly IInventoryService _inventoryService;

    #endregion

    #region Constructor

    public LoadLevelState(
      GameStateMachine gameStateMachine,
      ILoadScreen curtain,
      IGameLog gameLog,
      ISceneLoader sceneLoader,
      IGameFactory gameFactory,
      IUIFactory uIFactory,
      ICameraManager cameraManager,
      IAssetLoader assetLoader,
      IStaticDataService staticDataService,
      IPersistentProgressService persistentProgressService,
      ISaveLoadService saveLoadService,
      ISoundService soundService,
      IMusicPlayer musicPlayer,
      IPlayerWriter playerWriter,
      IPlayerReader playerReader,
      IBuffTrackerService buffTracker,
      IInventoryService inventoryService
      )
    {
      _logger = gameLog;
      _gameStateMachine = gameStateMachine;
      _curtain = curtain;

      _sceneLoader = sceneLoader;
      _gameFactory = gameFactory;
      _uiFactory = uIFactory;
      _cameraManager = cameraManager;
      _assetLoader = assetLoader;

      _staticData = staticDataService;
      _progressService = persistentProgressService;
      _saveLoadService = saveLoadService;
      _soundService = soundService;
      _musicPlayer = musicPlayer;

      _playerWriter = playerWriter;
      _playerReader = playerReader;

      _buffTracker = buffTracker;
      _inventoryService = inventoryService;
    }

    #endregion

    #region IGamePayloadedState

    public async void Enter(string payload)
    {
      try
      {
        _logger.Log("Entered state");

        StopLevelMusic();
        _curtain.Show();

        _assetLoader.Cleanup();
        _gameFactory.Cleanup();
        _uiFactory.Cleanup();

        _logger.Log("WarmUp started");
        await _gameFactory.WarmUp();
        _logger.Log("GameFactory WarmUp done");
        await _uiFactory.WarmUp();
        _logger.Log("UIFactory WarmUp done");
        await LoadLevelMusicAsync();
        _logger.Log("Level music loaded");

        await _sceneLoader.LoadPlatformBased(
          payload,
          _staticData.BuildConfig.TargetPlatform,
          onSceneLoaded: OnLevelLoadedAsync);

        _logger.Log("LoadAsync returned");
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"Entering state failed: {exception}");
        throw;
      }
    }

    public void Exit()
    {
      _logger.Log("Exited state");
      _curtain.Hide();
    }

    #endregion

    #region Level Loading

    private async void OnLevelLoadedAsync()
    {
      _logger.Log("Loading content for the active level...");

      try
      {
        await LoadLevelData();

        InitUIRoot();
        await InitGameWorldAsync();

        InformProgressReaders();
        SaveOnLoad();
        PlayLevelMusic();

        _gameStateMachine.EnterState<GameLoopState>();
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadLevel failed: {exception}");
      }
    }

    private async UniTask LoadLevelData()
    {
      _loadedSceneName = SceneManager.GetActiveScene().name;
      _levelData = await _staticData.LevelData.ForLevelAsync(_loadedSceneName);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    #endregion

    #region Music

    private void StopLevelMusic() => _musicPlayer.Stop();
    private void PlayLevelMusic() => _musicPlayer.Play();

    private async UniTask LoadLevelMusicAsync()
    {
      MusicPlaylist playlist = await _staticData.LevelMusic.ForLevelAsync(_loadedSceneName);
      _musicPlayer.SetPlaylist(playlist);
    }

    #endregion

    #region Game World

    private async UniTask InitGameWorldAsync()
    {
      GameObject player = await InitPlayerAsync();
      await InitSpawnersAsync();
      await InitHudAsync(player);
      await InitLevelTeleports();
    }

    #endregion

    #region Player

    private async UniTask<GameObject> InitPlayerAsync()
    {
      CleanupPlayer();

      GameObject player = await _gameFactory
        .CreateAndPlacePlayerAsync(GetPlayerCoordinates());

      _cameraManager.Follow(player);
      _playerWriter.SetPlayer(player);

      return player;
    }

    private void CleanupPlayer()
    {
      if (_playerReader.GetPlayer() != null)
       UObject.Destroy(_playerReader.GetPlayer());
    }

    private Coordinates GetPlayerCoordinates()
    {
      Coordinates playerSpawnCoords = _levelData.Teleports
        .Find(x => x.UniqueName == _progressService
          ?.Progress
          ?.PlayerWorldData
          ?.LastTeleportUniqueName)
        ?.PlayerSpawnCoords;

      return playerSpawnCoords ?? _levelData.PlayerStartCoordinates;
    }

    #endregion

    #region Spawners

    private async UniTask InitSpawnersAsync()
    {
      foreach (var spawnerData in _levelData.EnemySpawners)
      {
        _gameFactory.CreateEnemySpawner(
          spawnerData.Position,
          spawnerData.Rotation,
          spawnerData.SpawnerId,
          spawnerData.EnemyTypeId);

        await UniTask.Yield();
      }
    }

    #endregion

    #region HUD

    private async UniTask InitHudAsync(GameObject player)
    {
      CleanupHud();
      _hud = await _gameFactory.CreateHudAsync();
      _hud.GetComponent<PlayerUI>().Construct(player.GetComponent<IHealth>());
    }

    private void CleanupHud()
    {
      if (_hud != null)
        UObject.Destroy(_hud);
    }

    #endregion

    #region Teleports

    private async UniTask InitLevelTeleports()
    {
      foreach (var levelTeleport in _levelData.Teleports)
      {
        _gameFactory.CreateTeleport(
          coords: levelTeleport.Coords,
          scale: levelTeleport.Scale,
          levelKey: levelTeleport.LevelKey,
          uniqueName: levelTeleport.UniqueName);

        await UniTask.Yield();
      }
    }

    #endregion

    #region Progress

    private void InformProgressReaders()
    {
      foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
        progressReader.ReadProgress(_progressService.Progress);

      _buffTracker.ReadProgress(_progressService.Progress);
      _inventoryService.LoadFromSaveData(_progressService.Progress.Inventory);
      _soundService.ReadSettings(_progressService.SystemSettings);
    }

    private void SaveOnLoad() => _saveLoadService.SaveProgress();

    #endregion
  }
}
