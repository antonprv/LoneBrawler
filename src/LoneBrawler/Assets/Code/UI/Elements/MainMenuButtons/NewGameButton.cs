// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.MainMenuButtons
{
  public class NewGameButton : ZenjexBehaviour
  {
    public Button button;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly IStaticDataService _staticData;
    [Zenjex] private readonly ISaveLoadService _saveLoadService;
    [Zenjex] private readonly IGameStateMachine _gameStateMachine;

    protected override void OnAwake() =>
      button.onClick.AddListener(ResetGame);

    private void ResetGame()
    {
      _logger.Log("Resetting progress...");
      _progressService.Progress = InitNewProgress();
      _logger.Log("Progress reset.");
      SaveNewProgress();
      _logger.Log("Saving new progress.");
      StartGame();
      _logger.Log("Starting the game...");
    }

    private GameProgress InitNewProgress() =>
      new(_staticData.PlayerData, SceneAddresses.MainSceneAddress);

    private void SaveNewProgress() => _saveLoadService.SaveProgress();

    private void StartGame() =>
      _gameStateMachine.EnterState<LoadLevelState, string>(GetCurrentLevel());

    private string GetCurrentLevel() => _progressService.Progress.CurrentScene;
  }
}
