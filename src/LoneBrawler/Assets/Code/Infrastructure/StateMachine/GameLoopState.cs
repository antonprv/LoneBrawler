// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.StateMachine.States;

namespace Code.Infrastructure.StateMachine
{
  internal class GameLoopState : IGameState
  {
    private IGameLog _logger;
    private GameStateMachine _gameStateMachine;
    private ICoroutineRunner _runner;

    public GameLoopState(GameStateMachine gameStateMachine, ICoroutineRunner runner)
    {
      _logger = RootContext.Resolve<IGameLog>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;
    }

    public void Enter() => _logger.Log("Entered state");

    public void Exit() => _logger.Log("Exit state");
  }
}
