// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.Visuals;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.Services.DevConsole.Commands;
using Code.Infrastructure.Services.DevConsole.Commands.Gameplay;
using Code.Infrastructure.Services.DevConsole.Commands.Gameplay.Time;
using Code.Infrastructure.Services.DevConsole.Commands.Logs;
using Code.Infrastructure.Services.DevConsole.Commands.Performance;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;
using Code.Infrastructure.StateMachine.Interfaces;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.Services.DevConsole
{
  public class ConsoleComponent : MonoBehaviour, IGameInstanceComponent
  {
    private IBuildConfigSubservice _buildConfig;

    private IGameStateMachine _stateMachine;
    private IPlayerReader _playerReader;
    private FramerateManager _framerateManager;
    private IInputService _inputService;
    private ITimeService _timeService;
    private IPersistentProgressService _progressService;
    private IStaticDataService _staticData;
    private ISaveLoadService _saveLoad;

    private IDevConsole _console;

    public void DelayedAwake()
    {
      _stateMachine = RootContext.Resolve<IGameStateMachine>();
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _framerateManager = GetComponent<FramerateManager>();
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();
      _progressService = RootContext.Resolve<IPersistentProgressService>();
      _saveLoad = RootContext.Resolve<ISaveLoadService>();

      _staticData = RootContext.Resolve<IStaticDataService>();
      _buildConfig = _staticData.BuildConfig;

      _console = RootContext.Resolve<IDevConsole>();
      _console?.Initialize();

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
      _console.RegisterCommand(new FilterLogsCommand(_console));
      _console.RegisterCommand(new LogStatsCommand(_console));

      // PERFORMANCE
      _console.RegisterCommand(new SetFPSCommand(_console));
      if (_framerateManager != null)
        _console.RegisterCommand(new ToggleFPSCounterCommand(_console, _framerateManager));

      // GAMEPLAY
      _console.RegisterCommand(new LoadLevelCommand(
        _console, _staticData, _stateMachine, _saveLoad, _progressService, _playerReader));
      _console.RegisterCommand(new QuitToMenu(_console, _stateMachine));
      _console.RegisterCommand(new PlayerWarpCommand(_console, _playerReader));
      _console.RegisterCommand(new ResetGameCommand(
        _console, _progressService, _staticData, _saveLoad, _stateMachine));

      // GAMEPLAY | TIME
      _console.RegisterCommand(new PauseGameCommand(_console, _timeService, _inputService));

      // CONTROLFLOW
      _console.RegisterCommand(new ClearCommand(_console));
    }
  }
}
