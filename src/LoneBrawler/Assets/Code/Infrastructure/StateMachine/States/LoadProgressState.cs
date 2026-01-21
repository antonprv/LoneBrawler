// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data;
using Code.Infrastructure;
using Code.Infrastructure.Services.PersistentProgress;
using Code.Infrastructure.StateMachine;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.Infrastructure.Services.SaveLoad;

namespace Assets.Code.Infrastructure.StateMachine.States
{
  internal class LoadProgressState : IGameState, IStateDepsReader
  {
    private IGameLog _logger;
    private GameStateMachine _gameStateMachine;
    private IPersistentProgressService _progressService;
    private ISaveLoadService _saveLoadService;

    public LoadProgressState(GameStateMachine gameStateMachine)
    {
      _logger = RootContext.Resolve<IGameLog>();

      _gameStateMachine = gameStateMachine;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      LoadProgressOrInitNew();

      _logger.Log($"Transitioning to state {nameof(LoadLevelState)}");
      _gameStateMachine.EnterState<LoadLevelState, string>
        (_progressService.Progress.WorldData.TransformOnLevel.LevelName);
    }

    public void ReadDependencies(GameStateDependencies gameStateDependencies)
    {
      _progressService = gameStateDependencies.progressService;
      _saveLoadService = gameStateDependencies.saveLoadService;
    }

    public void Exit() => _logger.Log("Exited state");


    private void LoadProgressOrInitNew()
    {
      _logger.Log("Loading player progress...");

      _progressService.Progress = _saveLoadService.LoadProgress() ?? NewProgress();
    }

    private PlayerProgress NewProgress()
    {
      // TODO: Move to config
      return new PlayerProgress("Main");
    }
  }
}
