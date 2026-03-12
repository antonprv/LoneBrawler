// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.DevConsole.Types;

using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

namespace Code.Infrastructure.DevConsole.Commands.Gameplay
{
  public class QuitToMenu : IConsoleCommand
  {
    private readonly IDevConsole _console;
    private readonly IGameStateMachine _stateMachine;

    public QuitToMenu(
      IDevConsole devConsole,
      IGameStateMachine stateMachine
      )
    {
      _console = devConsole;
      _stateMachine = stateMachine;
    }

    public string CommandName => "quit_to_menu";

    public string Description => "Return to the dedicated map with the MainMenu";

    public void Execute(string[] args)
    {
      _console.AddMessage($"Returning to dedicated main menu...", ConsoleMessageType.Success);
      _stateMachine.EnterState<MainMenuState>();
    }
  }
}
