// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Factory.Interfaces;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Services.CameraManager.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Elements.Player;

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
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IStaticDataService _staticDataService;
    private readonly IUIFactory _uiFactory;
    private readonly IPlayerWriter _playerWriter;

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

      _playerWriter = RootContext.Resolve<IPlayerWriter>();

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

      InitUIRoot();
      InitGameWorld();
      InformProgressReaders();

      _gameStateMachine.EnterState<GameLoopState>();
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
    }

    private void InitSpawners()
    {
      string sceneKey = SceneManager.GetActiveScene().name;
      LevelStaticData levelData = _staticDataService.LevelData.ForLevel(sceneKey);
      foreach (var enemySpawnerData in levelData.EnemySpawners)
      {
        _gameFactory.CreateEnemySpawner(
          enemySpawnerData.Position, enemySpawnerData.SpawnerId, enemySpawnerData.EnemyTypeId);
      }
    }

    private GameObject InitPlayer()
    {
      GameObject player = _gameFactory.CreateAndPlacePlayer();
      _cameraManager.Follow(player);
      _playerWriter.SetPlayer(player);

      return player;
    }

    private void InitHud(GameObject player)
    {
      GameObject hud = _gameFactory.CreateHud();
      hud.GetComponent<PlayerUI>()
        .Construct(player.GetComponent<PlayerHealth>());
    }
  }
}
