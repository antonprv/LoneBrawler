// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadProgress : IGameState
  {
    private readonly IGameLog _logger;

    private GameStateMachine _gameStateMachine;
    private IPersistentProgressService _progressService;
    private ISaveLoadService _saveLoadService;
    private IStaticDataService _staticData;

    public LoadProgress(GameStateMachine gameStateMachine)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _progressService = RootContext.Resolve<IPersistentProgressService>();
      _saveLoadService = RootContext.Resolve<ISaveLoadService>();
      _staticData = RootContext.Resolve<IStaticDataService>();

      _gameStateMachine = gameStateMachine;
    }

    public async void Enter()
    {
      _logger.Log("Entered state");

      try
      {
        await LoadProgressOrInitNew();

        _logger.Log($"Transitioning to state {nameof(LoadLevelState)}");
        _gameStateMachine.EnterState<MainMenuState>();
      }
      catch (System.Exception exception)
      {
        _logger.Log(LogType.Error, $"LoadProgress.Enter failed: {exception}");
      }
    }

    public void Exit() => _logger.Log("Exited state");

    private async Task LoadProgressOrInitNew()
    {
      _logger.Log("Loading player progress...");

      await _staticData.LoadGameDataAsync();

      _progressService.Progress = _saveLoadService.LoadProgress() ?? NewProgress();
      _saveLoadService.SaveProgress();
    }

    private GameProgress NewProgress() =>
      new GameProgress(_staticData.PlayerData, SceneAddresses.MainSceneAddress);
  }
}
