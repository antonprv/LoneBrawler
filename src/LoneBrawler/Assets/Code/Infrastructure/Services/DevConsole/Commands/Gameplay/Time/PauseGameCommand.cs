// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.DevConsole.Types;

using Code.Infrastructure.Services.Time;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time
{
  public class PauseGameCommand : IConsoleCommand
  {
    private IDevConsole _console;
    private ITimeService _timeService;

    public string CommandName => "pause_game";

    public string Description =>
      "Only pauses objects, relying on time service. " +
      "Or pause everything fully if set to full. " +
      "Usage: pause_game <true|false|full>";

    public PauseGameCommand(IDevConsole console, ITimeService timeService)
    {
      _console = console;
      _timeService = timeService;
    }

    public void Execute(string[] args)
    {
      if (args.Length == 0)
        _console.AddMessage(Description, ConsoleMessageType.Warning);

      switch (args[0])
      {
        case "true":
          PauseGame(true);
          return;
        case "false":
          PauseGame(false);
          return;
        case "full":
          PauseGame(true, fullPause: true);
          return;
        default:
          break;
      }

      _console.AddMessage(Description, ConsoleMessageType.Warning);
    }

    private void PauseGame(bool v, bool fullPause = false)
    {
      if (!v) fullPause = false;

      if (v)
        _timeService.StopTime();
      else if (!v)
        _timeService.StartTime();

      UnityEngine.Time.timeScale = fullPause ? 0 : 1;
    }
  }
}
