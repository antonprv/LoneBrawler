// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.Visuals;
using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;

namespace Code.Infrastructure.Services.DevConsole.Commands.Performance
{
  public class ToggleFPSCounterCommand : IConsoleCommand
  {
    private readonly FramerateManager _framerateManager;
    private readonly IDevConsole _console;

    public string CommandName => "togglefps";
    public string Description => "Toggle FPS counter visibility. Usage: togglefps";

    public ToggleFPSCounterCommand(IDevConsole console, FramerateManager framerateManager)
    {
      _console = console;
      _framerateManager = framerateManager;
    }

    public void Execute(string[] args)
    {
      if (_framerateManager == null)
      {
        _console.AddMessage("FramerateManager not found!", ConsoleMessageType.Error);
        return;
      }

      _framerateManager.showFPS = !_framerateManager.showFPS;
      _console.AddMessage($"FPS counter {(_framerateManager.showFPS ? "enabled" : "disabled")}", ConsoleMessageType.Success);
    }
  }
}
