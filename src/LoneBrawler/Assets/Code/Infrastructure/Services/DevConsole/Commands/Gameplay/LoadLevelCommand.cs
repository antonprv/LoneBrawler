// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay
{
  public class LoadLevelCommand : IConsoleCommand
  {
    private readonly IGameStateMachine _stateMachine;
    private readonly IDevConsole _console;

    public string CommandName => "loadlevel";
    public string Description => "Load a specific level. Usage: loadlevel <levelName>";

    public LoadLevelCommand(IDevConsole console, IGameStateMachine stateMachine)
    {
      _console = console;
      _stateMachine = stateMachine;
    }

    public void Execute(string[] args)
    {
      if (args.Length < 1)
      {
        _console.AddMessage(Description, ConsoleMessageType.Warning);
        return;
      }

      string levelName = args[0];
      _console.AddMessage($"Loading level: {levelName}", ConsoleMessageType.Log);
      _stateMachine.EnterState<LoadLevelState, string>(levelName);
    }
  }
}
