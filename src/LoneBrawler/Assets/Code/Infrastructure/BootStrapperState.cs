// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

using Reflex.Core;

using Unity.VisualScripting;

using UnityEngine;

namespace Code.Infrastructure
{
  public class BootStrapperState : IGameState
  {
    private IGameLog _logger;

    public BootStrapperState(GameStateMachine gameStateMachine)
    {
      _logger = RootContext.Resolve<IGameLog>();
    }

    public void Enter()
    {
      _logger.Log("Entered state");
    }

    public void Exit()
    {
      _logger.Log("ExitedState");
    }
  }
}
