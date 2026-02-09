// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;

namespace Code.Infrastructure.Services.DevConsole.Commands.ControlFlow
{
  public class ExitCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;

    public string CommandName => "exit";
    public string Description => "Close the console. Usage: exit";

    public ExitCommand(IDevConsole console)
    {
      _console = console;
    }

    public void Execute(string[] args)
    {
      _console.Toggle(); // Close the console
    }
  }
}
