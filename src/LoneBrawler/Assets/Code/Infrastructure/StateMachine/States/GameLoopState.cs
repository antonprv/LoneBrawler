// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.StateMachine.Types;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;

namespace Code.Infrastructure.StateMachine.States
{
  public class GameLoopState : IGameState
  {
    #region StateType
    public StateType Type => StateType.GameLoop;

    #endregion

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
