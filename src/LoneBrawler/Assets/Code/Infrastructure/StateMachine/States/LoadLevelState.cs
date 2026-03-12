// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Threading;

using Code.Infrastructure.StateMachine.Types;


#region Project-specifid imports

using Code.Common.CustomTypes.Infrastructure.Types;
using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Configs;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Elements.Player;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.InventoryService.Interfaces;

#endregion

using Cysharp.Threading.Tasks;

using UnityEngine;

using UObject = UnityEngine.Object;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;

namespace Code.Infrastructure.StateMachine.States
{
  public class LoadLevelState : IGamePayloadedState<string>
  {
    #region StateType

    public StateType Type => StateType.LoadLevel;

    #endregion

    #region State Infrastructure

    private readonly IGameLog _logger;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;

    #endregion

    #region State Data

    private GameObject _hud;
    private string _loadedSceneName;
    private LevelStaticData _levelData;
    private CancellationTokenSource _cts;

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
    private readonly IMusicPlayerHolder _musicPlayerHolder;

    #endregion

    #region Player

    private readonly IPlayerWriter _playerWriter;
    private readonly IPlayerReader _playerReader;

    #endregion

    #region Gameplay

    private readonly IBuffTrackerService _buffTracker;
    private readonly IInventoryService _inventoryService;
    private readonly ISoulsTrackerService _soulsTracker;

    #endregion

    #region Constructor

    /// <summary>
    /// All level loading and dependency logic
    /// </summary>
    public LoadLevelState(
      IGameStateMachine gameStateMachine,
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
      IMusicPlayerHolder musicPlayerHolder,
      IPlayerWriter playerWriter,
      IPlayerReader playerReader,
      IBuffTrackerService buffTracker,
      IInventoryService inventoryService,
      ISoulsTrackerService soulsTrackerService
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
      _musicPlayerHolder = musicPlayerHolder;

      _playerWriter = playerWriter;
      _playerReader = playerReader;

      _buffTracker = buffTracker;
      _inventoryService = inventoryService;

      _soulsTracker = soulsTrackerService;
    }

    #endregion

    #region IGamePayloadedState

    public void Enter(string payload) => EnterAsync(payload).Forget();

    private async UniTask EnterAsync(string payload)
    {
      _logger.Log("Entered state");

      _cts = new CancellationTokenSource();
      var ct = _cts.Token;

      _loadedSceneName = payload;

      try
      {
        StopLevelMusic();
        _curtain.Show();

        _gameFactory.Cleanup();
        _uiFactory.Cleanup();

        _logger.Log("WarmUp started");
        await _gameFactory.WarmUp();

        if (ct.IsCancellationRequested) return;

        _logger.Log("GameFactory WarmUp done");
        await _uiFactory.WarmUp();

        if (ct.IsCancellationRequested) return;

        _logger.Log("UIFactory WarmUp done");

        await _sceneLoader.LoadPlatformBased(
          payload,
          _staticData.BuildConfig.TargetPlatform,
          onSceneLoaded: () => OnLevelLoadedAsync(ct).Forget());

        _logger.Log("LoadAsync returned");
      }
      catch (OperationCanceledException)
      {
        _logger.Log("LoadLevelState.Enter cancelled");
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
      _cts?.Cancel();
      _cts?.Dispose();
      _cts = null;
      _curtain.Hide();
    }

    #endregion

    #region Level Loading

    private async UniTaskVoid OnLevelLoadedAsync(CancellationToken ct)
    {
      _logger.Log("Loading content for the active level...");

      try
      {
        await LoadLevelData(ct);

        if (ct.IsCancellationRequested) return;

        await LoadLevelMusicAsync(ct);

        if (ct.IsCancellationRequested) return;

        InitUIRoot();
        await InitGameWorldAsync(ct);

        if (ct.IsCancellationRequested) return;

        InformProgressReaders();
        SaveOnLoad();
        PlayLevelMusic().Forget();

        _gameStateMachine.EnterState<GameLoopState>();
      }
      catch (OperationCanceledException)
      {
        _logger.Log("LoadLevelState.OnLevelLoaded cancelled");
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadLevel failed: {exception}");
      }
    }

    private async UniTask LoadLevelData(CancellationToken ct)
    {
      _levelData = await _staticData.LevelData.ForLevelAsync(_loadedSceneName);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    #endregion

    #region Music

    private void StopLevelMusic()
    {
      if (_musicPlayerHolder.Current != null)
        _musicPlayerHolder.Current.Stop().Forget();
    }

    private async UniTaskVoid PlayLevelMusic()
    {
      if (_musicPlayerHolder.Current != null)
        await _musicPlayerHolder.Current.Play();
    }

    private async UniTask LoadLevelMusicAsync(CancellationToken ct)
    {
      MusicPlaylist playlist = await _staticData.LevelMusic.ForLevelAsync(_loadedSceneName);
      MusicPlayerConfig playerConfig = _staticData.MusicConfig.Confg;

      if (ct.IsCancellationRequested) return;

      if (_musicPlayerHolder.Current == null)
      {
        _logger.Log(LogType.Error, "MusicPlayer is not registered yet!");
        return;
      }

      _musicPlayerHolder.Current.SetConfig(playerConfig);
      _musicPlayerHolder.Current.SetPlaylist(playlist);
    }

    #endregion

    #region Game World

    private async UniTask InitGameWorldAsync(CancellationToken ct)
    {
      GameObject player = await InitPlayerAsync(ct);

      if (ct.IsCancellationRequested) return;

      await InitSpawnersAsync(ct);

      if (ct.IsCancellationRequested) return;

      await InitHudAsync(player, ct);

      if (ct.IsCancellationRequested) return;

      await InitLevelTeleports(ct);
    }

    #endregion

    #region Player

    private async UniTask<GameObject> InitPlayerAsync(CancellationToken ct)
    {
      CleanupPlayer();

      GameObject player = await _gameFactory
        .CreateAndPlacePlayerAsync(GetPlayerCoordinates());

      if (ct.IsCancellationRequested) return null;

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

    private async UniTask InitSpawnersAsync(CancellationToken ct)
    {
      foreach (var spawnerData in _levelData.EnemySpawners)
      {
        if (ct.IsCancellationRequested) return;

        _gameFactory.CreateEnemySpawner(
          spawnerData.Position,
          spawnerData.Rotation,
          spawnerData.SpawnerId,
          spawnerData.EnemyTypeId);

        await UniTask.Yield(ct);
      }
    }

    #endregion

    #region HUD

    private async UniTask InitHudAsync(GameObject player, CancellationToken ct)
    {
      CleanupHud();
      _hud = await _gameFactory.CreateHudAsync();

      if (ct.IsCancellationRequested) return;

      _hud.GetComponent<PlayerUI>().Construct(player.GetComponent<IHealth>());
    }

    private void CleanupHud()
    {
      if (_hud != null)
        UObject.Destroy(_hud);
    }

    #endregion

    #region Teleports

    private async UniTask InitLevelTeleports(CancellationToken ct)
    {
      foreach (var levelTeleport in _levelData.Teleports)
      {
        if (ct.IsCancellationRequested) return;

        _gameFactory.CreateTeleport(
          coords: levelTeleport.Coords,
          scale: levelTeleport.Scale,
          levelKey: levelTeleport.LevelKey,
          uniqueName: levelTeleport.UniqueName);

        await UniTask.Yield(ct);
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
      _soulsTracker.ReadProgress(_progressService.Progress);
    }

    private void SaveOnLoad() => _saveLoadService.SaveProgress();

    #endregion
  }
}
