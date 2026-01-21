// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Assets.Code.Infrastructure.StateMachine.States;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress;

namespace Code.Infrastructure.StateMachine.States
{
  public class BootStrapperState : IGameState
  {
    private const string InitialScene = "Initial";
    private const string MainScene = "Main";

    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;

    /// <summary>
    /// Mandatory class, initializes all other states dependencies
    /// </summary>
    /// <param name="gameStateMachine"></param>
    /// <param name="runner"></param>
    public BootStrapperState(
      GameStateMachine gameStateMachine,
      ICoroutineRunner runner)
    {
      _logger = RootContext.Resolve<IGameLog>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      InstallDependencies();

      _gameStateMachine.Dependencies.sceneLoader
        .Load(InitialScene, _runner, onSceneLoaded: EnterLoadLevel);
    }

    private void InstallDependencies()
    {
      _gameStateMachine.Dependencies =
        new GameStateDependencies(
          RootContext.Resolve<ISceneLoader>(),
          RootContext.Resolve<IGameFactory>(),
          RootContext.Resolve<ICameraManager>(),
          RootContext.Resolve<IPersistentProgressService>());
    }

    private void EnterLoadLevel()
    {
      _logger.Log($"Transitioning to {nameof(LoadProgressState)}");
      _gameStateMachine.EnterState<LoadProgressState>();
    }

    public void Exit() => _logger.Log("Exited state");

  }
}
