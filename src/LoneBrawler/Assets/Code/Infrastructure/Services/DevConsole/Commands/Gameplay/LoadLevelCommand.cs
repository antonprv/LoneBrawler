// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes;
using Code.Common.Extensions.CustomTypes.Types;
using Code.Data.StaticData;
using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

using UnityEngine;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay
{
  public class LoadLevelCommand : IConsoleCommand
  {
    private readonly IGameStateMachine _stateMachine;
    private readonly ISaveLoadService _saveLoad;
    private readonly IPersistentProgressService _progressService;
    private readonly IPlayerReader _playerReader;
    private readonly IDevConsole _console;
    private readonly IStaticDataService _staticData;
    private string _levelName;
    private LevelStaticData _levelData;

    public string CommandName => "load_level";
    public string Description => "Load a specific level. Usage: load_level <levelName>";

    public LoadLevelCommand(
      IDevConsole console,
      IStaticDataService staticData,
      IGameStateMachine stateMachine,
      ISaveLoadService saveLoadService,
      IPersistentProgressService progressService,
      IPlayerReader playerReader)
    {

      _console = console;
      _staticData = staticData;
      _stateMachine = stateMachine;
      _saveLoad = saveLoadService;
      _progressService = progressService;
      _playerReader = playerReader;
    }

    public void Execute(string[] args)
    {
      if (args.Length < 1)
      {
        _console.AddMessage(Description, ConsoleMessageType.Warning);
        return;
      }

      _levelName = args[0];
      _console.AddMessage($"[Console] Loading level: {_levelName}", ConsoleMessageType.Log);

      _levelData = GetCurrentLevelData();

      if (_levelData == null)
        _console.AddMessage($"Could not find {_levelName} level.", ConsoleMessageType.Error);

      SaveGame();
      SetPlayerSpawnCoordinates();
      LoadGame();
      LoadLevel(_levelName);
    }

    private LevelStaticData GetCurrentLevelData() =>
       _staticData.LevelData?.ForLevel(_levelName);

    private void SetPlayerSpawnCoordinates() =>
      _progressService
      .Progress
      .PlayerWorldData
      .TransformOnLevel
      .Transform = GetPlayerSpawnPoint();

    private TransformData GetPlayerSpawnPoint() =>
      _levelData.PlayerStartCoordinates.AsTransformData(GetPlayerScale());

    private Vector3 GetPlayerScale() =>
      _playerReader.GetPlayer().transform.localScale;

    private void SaveGame() => _saveLoad.SaveProgress();

    private void LoadGame() =>
      _progressService.Progress = _saveLoad.LoadProgress();

    private void LoadLevel(string levelName) =>
      _stateMachine.EnterState<LoadLevelState, string>(levelName);
  }
}
