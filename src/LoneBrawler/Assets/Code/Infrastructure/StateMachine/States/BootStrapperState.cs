// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Assets.Code.Gameplay.Services.SceneLoader;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

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

    public BootStrapperState(GameStateMachine gameStateMachine, ICoroutineRunner runner)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _sceneLoader = RootContext.Resolve<ISceneLoader>();
      
      _gameStateMachine = gameStateMachine;
      _runner = runner;
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
