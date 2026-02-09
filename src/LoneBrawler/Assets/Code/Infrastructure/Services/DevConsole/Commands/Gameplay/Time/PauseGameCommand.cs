// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Services.Time;
using Code.Infrastructure.Services.DevConsole.Interfaces;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time
{
  public class PauseGameCommand : IConsoleCommand
  {
    private IDevConsole _console;
    private ITimeService _timeService;

    public string CommandName => "pausegame";

    public string Description =>
      "Only pauses objects, relying on time service." +
      " Usage: pausegame <true|false>";

    public PauseGameCommand(IDevConsole console, ITimeService timeService)
    {
      _console = console;
      _timeService = timeService;
    }

    public void Execute(string[] args)
    {
      if (args.Length == 0)
        _console.AddMessage(Description, ConsoleMessageType.Warning);

      if (args[0] == "true")
        PauseGame(true);
      else if (args[0] == "fase")
        PauseGame(false);
      else
        _console.AddMessage(Description, ConsoleMessageType.Warning);
    }

    private void PauseGame(bool v)
    {
      if (v)
        _timeService.StopTime();
      else if (!v)
        _timeService.StartTime();
    }
  }
}
