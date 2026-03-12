// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.DevConsole.Interfaces;

#region Dependencies

using Code.Gameplay.Utils.Visuals;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerPrefs.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;
using Code.Infrastructure.StateMachine.Interfaces;

#endregion

#region Commands

using Code.Infrastructure.DevConsole.Commands.Logs;
using Code.Infrastructure.DevConsole.Commands.Performance;
using Code.Infrastructure.DevConsole.Commands.Gameplay;
using Code.Infrastructure.DevConsole.Commands.Gameplay.Time;
using Code.Infrastructure.DevConsole.Commands;

#endregion

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Infrastructure.DevConsole
{
  public class ConsoleComponent : ZenjexBehaviour, IConsoleComponent
  {
    [Zenjex] private readonly IBuildConfigSubservice _buildConfig;
    [Zenjex] private readonly ISoulsTrackerService _soulsTracker;
    [Zenjex] private readonly IGameStateMachine _stateMachine;
    [Zenjex] private readonly IPlayerReader _playerReader;
    [Zenjex] private readonly FramerateManager _framerateManager;
    [Zenjex] private readonly IInputService _inputService;
    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly IStaticDataService _staticData;
    [Zenjex] private readonly ISaveLoadService _saveLoad;
    [Zenjex] private readonly IDevConsole _console;
    [Zenjex] private readonly IPlayerPrefsService _playerPrefs;

    public void InitializeCommands()
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
      _console.RegisterCommand(new ResetGameCommand(_console, _stateMachine, _playerPrefs));
      _console.RegisterCommand(new AddSoulsCommand(_console, _soulsTracker));

      // GAMEPLAY | TIME
      _console.RegisterCommand(new PauseGameCommand(_console, _timeService, _inputService));

      // CONTROLFLOW
      _console.RegisterCommand(new ClearCommand(_console));
    }
  }
}
