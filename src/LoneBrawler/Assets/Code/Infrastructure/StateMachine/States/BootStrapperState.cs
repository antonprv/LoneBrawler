// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.SceneLoader;

namespace Code.Infrastructure.StateMachine.States
{
  public class BootStrapperState : IGameState
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private ISceneLoader _sceneLoader;

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
      _sceneLoader = RootContext.Resolve<ISceneLoader>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      // TODO: move to config file
      _sceneLoader.Load("Initial", _runner, onSceneLoaded: EnterLoadLevel);
    }

    private void EnterLoadLevel()
    {
      _logger.Log($"Transitioning to {nameof(LoadProgressState)}");
      _gameStateMachine.EnterState<LoadProgressState>();
    }

    public void Exit() => _logger.Log("Exited state");
  }
}
