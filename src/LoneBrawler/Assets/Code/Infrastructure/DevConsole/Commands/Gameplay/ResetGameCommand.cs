// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.DevConsole.Types;

using Code.Infrastructure.Services.PlayerPrefs.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

namespace Code.Infrastructure.DevConsole.Commands.Gameplay
{
  public class ResetGameCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;
    private readonly IGameStateMachine _stateMachine;
    private readonly IPlayerPrefsService _playerPrefs;

    public ResetGameCommand(
      IDevConsole console,
      IGameStateMachine stateMachine,
      IPlayerPrefsService playerPrefs
      )
    {
      _console = console;
      _stateMachine = stateMachine;
      _playerPrefs = playerPrefs;
    }

    public string CommandName => "reset_game";

    public string Description =>
      "Wipes all save data from PlayerPrefs and restarts game. Usage: reset_game";

    public void Execute(string[] args)
    {
      ClearPrefs();
      _console.AddMessage($"Game progress wiped.", ConsoleMessageType.Success);
      _console.AddMessage($"Restarting the game.", ConsoleMessageType.Log);
      Restart();
    }

    private void ClearPrefs()
    {
      _playerPrefs.DeleteAll();
      _playerPrefs.Save();
    }

    private void Restart() => _stateMachine.EnterState<BootStrapperState>();
  }
}

