// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Visuals.UI;
using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure.Factory;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress;
using Code.Infrastructure.Services.PlayerProvider;

using UnityEngine;

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

      InitGameWorld();
      InformProgressReaders();

      _gameStateMachine.EnterState<GameLoopState>();
    }

    private void InformProgressReaders()
    {
      foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
        progressReader.ReadProgress(_persistentProgressService.Progress);
    }

    private void InitGameWorld()
    {
      GameObject player = _gameFactory.CreateAndPlacePlayer();
      _cameraManager.Follow(player);
      _gameFactory.CreateHud();

      _playerWriter.SetPlayer(player);
    }
  }
}
