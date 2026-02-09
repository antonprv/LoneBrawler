// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Commands.Gameplay;

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Visuals;
using Code.Infrastructure.Installer;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.Services.DevConsole.Commands.ControlFlow;
using Code.Infrastructure.Services.DevConsole.Commands.Gameplay;
using Code.Infrastructure.Services.DevConsole.Commands.Logs;
using Code.Infrastructure.Services.DevConsole.Commands.Performance;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;

using UnityEngine;
using Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Gameplay.Services.Time;

namespace Code.Infrastructure.Services.DevConsole
{
  public class ConsoleComponent : MonoBehaviour, IGameInstanceComponent
  {
    private IBuildConfigSubservice _buildConfig;
    private IDevConsole _console;
    private IGameStateMachine _stateMachine;
    private IPlayerReader _playerReader;
    private FramerateManager _framerateManager;
    private IInputService _inputService;
    private ITimeService _timeService;
    private GameInstance _gameInstance;

    public void RegisterGameInstance(GameInstance gameInstance) =>
      _gameInstance = gameInstance;

    public void DelayedAwake()
    {
      _buildConfig = RootContext.Resolve<IBuildConfigSubservice>();
      _console = RootContext.Resolve<IDevConsole>();
      _stateMachine = RootContext.Resolve<IGameStateMachine>();
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _framerateManager = GetComponent<FramerateManager>();
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();

      InitializeConsoleCommands();
    }

    private void InitializeConsoleCommands()
    {
      if (!_buildConfig.IsDevelopment()) return;


      if (_console == null)
        return;

      // LOGS
      _console.RegisterCommand(new ToggleUnityLogsCommand(_console));
      _console.RegisterCommand(new ExportLogsCommand(_console));
      //_console.RegisterCommand(new FilterLogsCommand(_console));
      _console.RegisterCommand(new LogStatsCommand(_console));

      // PERFORMANCE
      _console.RegisterCommand(new SetFPSCommand(_console));
      if (_framerateManager != null)
        _console.RegisterCommand(new ToggleFPSCounterCommand(_console, _framerateManager));

      // GAMEPLAY
      _console.RegisterCommand(new LoadLevelCommand(_console, _stateMachine));
      _console.RegisterCommand(new PlayerWarpCommand(_console, _playerReader));
      _console.RegisterCommand(new WipeSaveCommand(_console));

      // GAMEPLAY | TIME
      _console.RegisterCommand(new FreezeGameCommand(_console, _inputService));
      _console.RegisterCommand(new PauseGameCommand(_console, _timeService));

      // CONTROLFLOW
      _console.RegisterCommand(new ClearCommand(_console));
      _console.RegisterCommand(new ExitCommand(_console));
    }
  }
}
