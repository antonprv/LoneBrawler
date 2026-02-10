// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

using UnityEngine;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay
{
  public class RestartGameCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;
    private readonly IPersistentProgressService _progressService;
    private readonly IStaticDataService _staticDataService;
    private readonly ISaveLoadService _saveLoad;
    private readonly IGameStateMachine _stateMachine;

    public RestartGameCommand(
      IDevConsole console,
      IPersistentProgressService progressService,
      IStaticDataService staticDataService,
      ISaveLoadService saveLoad,
      IGameStateMachine stateMachine
      )
    {
      _console = console;
      _progressService = progressService;
      _staticDataService = staticDataService;
      _saveLoad = saveLoad;
      _stateMachine = stateMachine;
    }

    public string CommandName => "restart_game";

    public string Description =>
      "Wipes all save data from PlayerPrefs and restarts game. Usage: restart_game";

    public void Execute(string[] args)
    {
      ClearPrefs();
      Restart();
      _console.AddMessage($"Game progress wiped.", ConsoleMessageType.Log);
    }

    private void ClearPrefs()
    {
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();
    }

    private void Restart() => _stateMachine.EnterState<BootStrapperState>();
  }
}

