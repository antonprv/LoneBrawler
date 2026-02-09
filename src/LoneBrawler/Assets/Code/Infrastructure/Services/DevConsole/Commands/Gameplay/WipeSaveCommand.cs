// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay
{
  public class WipeSaveCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;

    public WipeSaveCommand(IDevConsole console)
    {
      _console = console;
    }

    public string CommandName => "wipesave";

    public string Description => "Wipes all save data from PlayerPrefs. Usage: wipesave";

    public void Execute(string[] args)
    {
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();

      _console.AddMessage($"Cleared all player save data.", ConsoleMessageType.Log);
    }
  }
}

