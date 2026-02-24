// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  internal class GameLoopState : IGameState
  {
    private readonly IGameLog _logger;

    public GameLoopState() =>
      _logger = RootContext.Resolve<IGameLog>();

    public void Enter() => _logger.Log("Entered state");

    public void Exit() => _logger.Log("Exit state");
  }
}
