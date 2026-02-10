// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes.Types;

using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Player;
using Code.UI.Elements.Utils.LoadingScreen.Interfaces;
using Code.UI.Factory.Interfaces;
using Code.Utils.Extensions.Async;
using Code.Utils.Extensions.Logging;
using Code.Utils.Extensions.ReflexExtensions;

using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool _wasHudCreated;
    private GameObject _hud;
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IStaticDataService _staticDataService;
    private readonly IUIFactory _uiFactory;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IPlayerWriter _playerWriter;
    private readonly IPlayerReader _playerReader;

    public LoadLevelState(
      GameStateMachine gameStateMachine,
      ICoroutineRunner runner,
      ILoadScreen curtain)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _sceneLoader = RootContext.Resolve<ISceneLoader>();
      _gameFactory = RootContext.Resolve<IGameFactory>();
      _cameraManager = RootContext.Resolve<ICameraManager>();
      _persistentProgressService = RootContext.Resolve<IPersistentProgressService>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();
      _uiFactory = RootContext.Resolve<IUIFactory>();
      _saveLoadService = RootContext.Resolve<ISaveLoadService>();

      _playerWriter = RootContext.Resolve<IPlayerWriter>();
      _playerReader = RootContext.Resolve<IPlayerReader>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;

      _curtain = curtain;
    }

    public void Enter(string payload)
    {
      _logger.Log("Entered state");

      _curtain.Show();
      _gameFactory.Cleanup();
      _sceneLoader.Load(payload, _runner, onSceneLoaded: OnLevelLoaded);
    }
    public void Exit()
    {
      _logger.Log("Exited state");

      _curtain.Hide();
    }

    private void OnLevelLoaded()
    {
      _logger.Log("Loading content for the active level...");

      LoadLevelData();

      InitUIRoot();
      InitGameWorld();
      InformProgressReaders();

      MakeFirstSave();

      _gameStateMachine.EnterState<GameLoopState>();
    }

    private void MakeFirstSave() => _saveLoadService.SaveProgress();

    private void LoadLevelData()
    {
      _loadedSceneName = SceneManager.GetActiveScene().name;
      _levelData = _staticDataService.LevelData.ForLevel(_loadedSceneName);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRoot();

    private void InformProgressReaders()
    {
      foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
        progressReader.ReadProgress(_persistentProgressService.Progress);
    }

    private void InitGameWorld()
    {
      GameObject player = InitPlayer();
      InitSpawners();
      InitHud(player);
      InitLevelTeleports();
    }

    private GameObject InitPlayer()
    {
      CleanupPlayer();

      GameObject player = _gameFactory
        .CreateAndPlacePlayer(GetPlayerCoordinates());

      _cameraManager.Follow(player);
      _playerWriter.SetPlayer(player);

      return player;
    }

    private void CleanupPlayer()
    {
      if (_playerReader.GetPlayer() != null)
        Object.Destroy(_playerReader.GetPlayer());
    }

    private Coordinates GetPlayerCoordinates()
    {
      Coordinates playerSpawnCoords = _levelData.Teleports
        .Find(
          x => x.UniqueName == _persistentProgressService
          ?.Progress
          ?.PlayerWorldData
          ?.LastTeleportUniqueName
        )
        ?.PlayerSpawnCoords;

      return playerSpawnCoords ?? _levelData.PlayerStartCoordinates;
    }

    private void InitSpawners()
    {
      //CleanupSpawners();
      foreach (var enemySpawnerData in _levelData.EnemySpawners)
      {
        _gameFactory.CreateEnemySpawner(
          enemySpawnerData.Position, enemySpawnerData.SpawnerId, enemySpawnerData.EnemyTypeId);
      }
    }

    private void InitHud(GameObject player)
    {
      CleanupHud();
      _hud = _gameFactory.CreateHud();
      _hud.GetComponent<PlayerUI>()
        .Construct(player.GetComponent<PlayerHealth>());
    }

    private void CleanupHud()
    {
      if (_hud != null)
        GameObject.Destroy(_hud);
    }

    private void InitLevelTeleports()
    {
      foreach (var levelTeleport in _levelData.Teleports)
        _gameFactory.CreateTeleport(
          coords: levelTeleport.Coords,
          scale: levelTeleport.Scale,
          levelKey: levelTeleport.LevelKey,
          uniqueName: levelTeleport.UniqueName
          );
    }

  }
}
