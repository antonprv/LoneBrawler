// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StateMachine.States
{
  internal class GameLoopState : IGameState
  {
    private readonly IGameLog _logger;
    private readonly ILiveProgressSync _progressSync;
    private readonly IInputService _inputSercvice;

    /// <summary>
    /// Placeholder state to let GSM know that the game is running.
    /// </summary>
    public GameLoopState(
      IGameLog gameLog,
      ILiveProgressSync liveProgressSync,
      IInputService inputService
      )
    {
      _logger = gameLog;
      _progressSync = liveProgressSync;
      _inputSercvice = inputService;
    }

    public void Enter()
    {
      _logger.Log("Entered state");
      _inputSercvice.GameInputEnabled = true;
      _progressSync.StartSyncLoop();
    }

    public void Exit()
    {
      _logger.Log("Exit state");
      _inputSercvice.GameInputEnabled = false;
      _progressSync.StopSyncLoop();
    }
  }
}
