// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.DevConsole.Types;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.Time;

using YG;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time
{
  public class PauseGameCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;
    private readonly ITimeService _timeService;
    private readonly IInputService _inputService;

    public string CommandName => "pause_game";

    public string Description =>
      "Only pauses objects, relying on time service. " +
      "Or pause everything fully if set to full. " +
      "Usage: pause_game <true|false>";

    public PauseGameCommand(IDevConsole console, ITimeService timeService, IInputService inputService)
    {
      _console = console;
      _timeService = timeService;
      _inputService = inputService;
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

    private void PauseGame(bool paused)
    {
      if (paused)
        _timeService.StopTime();
      else if (!paused)
        _timeService.StartTime();

      YG2.PauseGame(paused);
      _inputService.GameInputEnabled = !paused;
    }
  }
}
