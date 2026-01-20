// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Visuals.UI;
using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.SceneLoader;

using UnityEngine;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadLevelState : IGamePayloadedState<string>
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;

    private readonly IGameLog _logger;
    private readonly ISceneLoader _sceneLoader;
    private readonly IGameFactory _gameFactory;
    private readonly ICameraManager _cameraManager;

    public LoadLevelState(
      GameStateMachine gameStateMachine,
      ICoroutineRunner runner,
      ISceneLoader sceneLoader,
      IGameFactory gameFactory,
      ICameraManager cameraManager,
      ILoadScreen curtain)
    {
      _logger = RootContext.Resolve<IGameLog>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;
      _sceneLoader = sceneLoader;
      _gameFactory = gameFactory;
      _cameraManager = cameraManager;

      _curtain = curtain;
    }

    public void Enter(string payload)
    {
      _logger.Log("Entered state");

      _curtain.Show();
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

      GameObject player = _gameFactory.CreateAndPlacePlayer();
      _cameraManager.Follow(player);
      _gameFactory.CreateHud();

      _gameStateMachine.EnterState<GameLoopState>();
    }
  }
}
