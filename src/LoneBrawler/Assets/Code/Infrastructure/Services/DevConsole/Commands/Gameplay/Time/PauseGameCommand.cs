// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.DevConsole.Types;

using Code.Infrastructure.Services.Time;

using YG;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time
{
  public class PauseGameCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;
    private readonly ITimeService _timeService;

    public string CommandName => "pause_game";

    public string Description =>
      "Only pauses objects, relying on time service. " +
      "Or pause everything fully if set to full. " +
      "Usage: pause_game <true|false>";

    public PauseGameCommand(IDevConsole console, ITimeService timeService)
    {
      _console = console;
      _timeService = timeService;
    }

    public void Execute(string[] args)
    {
      if (args.Length != 1)
      {
        _console.AddMessage(Description, ConsoleMessageType.Warning);
        return;
      }

      switch (args[0])
      {
        case "true":
          PauseGame(true);
          return;
        case "false":
          PauseGame(false);
          return;
        default:
          break;
      }

      _console.AddMessage(Description, ConsoleMessageType.Warning);
    }

    private void PauseGame(bool v)
    {
      if (v)
        _timeService.StopTime();
      else if (!v)
        _timeService.StartTime();

      YG2.PauseGame(v);
    }
  }
}
