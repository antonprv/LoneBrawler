// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.DevConsole.Types;
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
      " Usage: freeze_game <true|false|behaviours|events>";

    public FreezeGameCommand(IDevConsole console, IInputService inputService)
    {
      _console = console;
      _inputService = inputService;
    }

    public void Execute(string[] args)
    {
      if (args.Length == 0)
        _console.AddMessage(Description, ConsoleMessageType.Warning);

      switch (args[0])
      {
        case "true":
          SetFreeze(v: true);
          return;
        case "false":
          SetFreeze(v: false);
          return;
        case "behaviours":
          SetFreeze(v: true, freezeBehaviours: true);
          return;
        case "events":
          SetFreeze(v: true, freezeEvents: true);
          return;
        default:
          break;
      }

      _console.AddMessage(Description, ConsoleMessageType.Warning);
    }

    private void SetFreeze(
      bool v,
      bool freezeBehaviours = false,
      bool freezeEvents = false
      )
    {
      if (!v)
      {
        freezeBehaviours = false;
        freezeEvents = false;
      }

      UnityEngine.Time.timeScale = v ? 1 : 0;

      AudioListener.pause = v;

      _inputService.GameInputEnabled = v;

      EventSystem.current.enabled = freezeEvents;

      if (!freezeBehaviours) return;
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
