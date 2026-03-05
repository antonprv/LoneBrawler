// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.CustomTypes.Infrastructure.Types;
using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Data.StaticData;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Elements.Player;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.InventoryService.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadLevelState : IGamePayloadedState<string>
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;

    private ISceneLoader _sceneLoader;
    private IGameFactory _gameFactory;
    private ICameraManager _cameraManager;
    private string _loadedSceneName;
    private LevelStaticData _levelData;
    private GameObject _hud;
    private readonly IPersistentProgressService _progressService;
    private readonly IStaticDataService _staticData;
    private readonly IUIFactory _uiFactory;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IAssetLoader _assetLoader;
    private readonly IPlayerWriter _playerWriter;
    private readonly IPlayerReader _playerReader;
    private readonly IBuffTrackerService _buffTracker;
    private readonly IInventoryService _inventoryService;

    public LoadLevelState(
      GameStateMachine gameStateMachine,
      ILoadScreen curtain)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _cameraManager = RootContext.Resolve<ICameraManager>();

      _sceneLoader = RootContext.Resolve<ISceneLoader>();

      _progressService = RootContext.Resolve<IPersistentProgressService>();
      _staticData = RootContext.Resolve<IStaticDataService>();

      _gameFactory = RootContext.Resolve<IGameFactory>();
      _uiFactory = RootContext.Resolve<IUIFactory>();

      _saveLoadService = RootContext.Resolve<ISaveLoadService>();

      _assetLoader = RootContext.Resolve<IAssetLoader>();

      _playerWriter = RootContext.Resolve<IPlayerWriter>();
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _buffTracker = RootContext.Resolve<IBuffTrackerService>();

      _inventoryService = RootContext.Resolve<IInventoryService>();

      _gameStateMachine = gameStateMachine;

      _curtain = curtain;
    }

    public async void Enter(string payload)
    {
      try
      {
        _logger.Log("Entered state");

        _curtain.Show();

        _assetLoader.Cleanup();
        _gameFactory.Cleanup();
        _uiFactory.Cleanup();

        _logger.Log("WarmUp started");
        await _gameFactory.WarmUp();
        _logger.Log("GameFactory WarmUp done");
        await _uiFactory.WarmUp();
        _logger.Log("UIFactory WarmUp done");

        await _sceneLoader.LoadPlatformBased(
          payload,
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
        await LoadLevelData();

        InitUIRoot();
        await InitGameWorldAsync();

        InformProgressReaders();

        SaveOnLoad();

        _gameStateMachine.EnterState<GameLoopState>();
      }
      catch (Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadLevel failed: {exception}");
      }
    }

    private void SaveOnLoad() => _saveLoadService.SaveProgress();

    private async UniTask LoadLevelData()
    {
      _loadedSceneName = SceneManager.GetActiveScene().name;
      _levelData = await _staticData.LevelData.ForLevelAsync(_loadedSceneName);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    private void InformProgressReaders()
    {
      foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
        progressReader.ReadProgress(_progressService.Progress);

      _buffTracker.ReadProgress(_progressService.Progress);
      _inventoryService.LoadFromSaveData(_progressService.Progress.Inventory);
    }

    private async UniTask InitGameWorldAsync()
    {
      GameObject player = await InitPlayerAsync();
      await InitSpawnersAsync();
      await InitHudAsync(player);
      await InitLevelTeleports();
    }

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
        UnityEngine.Object.Destroy(_playerReader.GetPlayer());
    }

    private Coordinates GetPlayerCoordinates()
    {
      Coordinates playerSpawnCoords = _levelData.Teleports
        .Find(
          x => x.UniqueName == _progressService
          ?.Progress
          ?.PlayerWorldData
          ?.LastTeleportUniqueName
        )
        ?.PlayerSpawnCoords;

      return playerSpawnCoords ?? _levelData.PlayerStartCoordinates;
    }

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

    private async UniTask InitHudAsync(GameObject player)
    {
      CleanupHud();
      _hud = await _gameFactory.CreateHudAsync();
      _hud.GetComponent<PlayerUI>()
        .Construct(player.GetComponent<IHealth>());
    }

    private void CleanupHud()
    {
      if (_hud != null)
        GameObject.Destroy(_hud);
    }

    private async UniTask InitLevelTeleports()
    {
      foreach (var levelTeleport in _levelData.Teleports)
      {
        _gameFactory.CreateTeleport(
          coords: levelTeleport.Coords,
          scale: levelTeleport.Scale,
          levelKey: levelTeleport.LevelKey,
          uniqueName: levelTeleport.UniqueName
          );

        await UniTask.Yield();
      }
    }
  }
}
