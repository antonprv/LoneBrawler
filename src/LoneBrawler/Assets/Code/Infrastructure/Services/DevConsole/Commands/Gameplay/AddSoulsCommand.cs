// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.DevConsole.Types;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay
{
  class AddSoulsCommand : IConsoleCommand
  {
    private IDevConsole _console;
    private ISoulsTrackerService _soulsTracker;

    public string CommandName => "add_souls";
    public string Description => "Adds selected amount of souls to the player. Usage: add_souls <amount>";

    public AddSoulsCommand(IDevConsole console, ISoulsTrackerService soulsTracker)
    {
      _console = console;
      _soulsTracker = soulsTracker;
    }

    public void Execute(string[] args)
    {
      if (args.Length != 1)
      {
        _console.AddMessage(Description, ConsoleMessageType.Warning);
        return;
      }

      if (int.TryParse(args[0], out int souls))
      {
        _soulsTracker.AddSouls(souls);
      }
      else
      {
        _console.AddMessage("Entered invalid souls number!", ConsoleMessageType.Error);
        return;
      }
    }
  }
}
