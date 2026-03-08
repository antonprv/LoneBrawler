// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  internal class GameLoopState : IGameState
  {
    private readonly IGameLog _logger;
    private readonly ILiveProgressSync _progressSync;

    public GameLoopState(
      IGameLog gameLog,
      ILiveProgressSync liveProgressSync
      )
    {
      _logger = gameLog;
      _progressSync = liveProgressSync;
    }

    public void Enter()
    {
      _logger.Log("Entered state");
      _progressSync.StartSyncLoop();
    }

    public void Exit()
    {
      _logger.Log("Exit state");
      _progressSync.StopSyncLoop();
    }
  }
}
