// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time
{
  class FreezeGameCommand : IConsoleCommand
  {
    private IDevConsole _console;
    private IInputService _inputService;

    public string CommandName => "freeze_game";

    public string Description =>
      "Completely freeze all unity systems and game time." +
      " Warning! May cause errors," +
      " recommended use is to pause time to export logs through console." +
      " Usage: freeze_game <true|false|withbehaviours>";

    public FreezeGameCommand(IDevConsole console, IInputService inputService)
    {
      _console = console;
      _inputService = inputService;
    }

    public void Execute(string[] args)
    {
      if (args.Length == 0)
        _console.AddMessage(Description, ConsoleMessageType.Warning);

      if (args[0] == "true")
        SetFreeze(true);
      else if (args[0] == "false")
        SetFreeze(false);
      else if (args[0] == "withbehaviours")
        SetFreeze(true, true);
      else
        _console.AddMessage(Description, ConsoleMessageType.Warning);
    }

    private void SetFreeze(bool v, bool freezeBehaviours = false)
    {
      UnityEngine.Time.timeScale = v ? 1 : 0;

      AudioListener.pause = v;

      _inputService.GameInputEnabled = v;

      EventSystem.current.enabled = v;

      foreach (var mb in GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
      {
        mb.enabled = !freezeBehaviours;
        if (!v)
          mb.GetComponent<IDeactivatable>()?.Deactivate();
        else if (v)
          mb.GetComponent<IActivatable>()?.Activate();
      }
    }
  }
}
