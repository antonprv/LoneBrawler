// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.SceneLoader;

using Reflex.Attributes;

namespace Code.Infrastructure.StateMachine.States
{
  public class BootStrapperState : IGameState
  {
    private const string InitialScene = "Initial";
    private const string MainScene = "Main";

    private readonly IGameLog _logger;
    private readonly ISceneLoader _sceneLoader;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;

    public BootStrapperState(
      GameStateMachine gameStateMachine,
      ICoroutineRunner runner,
      ISceneLoader sceneLoader)
    {
      _logger = RootContext.Resolve<IGameLog>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;
      _sceneLoader = sceneLoader;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      _sceneLoader.Load(InitialScene, _runner, onSceneLoaded: EnterLoadLevel);
    }

    private void EnterLoadLevel()
    {
      _logger.Log($"Transitioning to {nameof(LoadLevelState)}");
      _gameStateMachine.EnterState<LoadLevelState, string>(MainScene);
    }

    public void Exit() => _logger.Log("Exited state");

  }
}
