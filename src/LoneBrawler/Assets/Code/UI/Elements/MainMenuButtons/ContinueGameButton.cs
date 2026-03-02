// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.MainMenuButtons
{
  public class ContinueGameButton : ZenjexBehaviour
  {
    public Button button;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly ISaveLoadService _saveLoadService;
    [Zenjex] private readonly IGameStateMachine _gameStateMachine;

    protected override void OnAwake() =>
      button.onClick.AddListener(ContinueGame);

    private void ContinueGame()
    {
      _logger.Log("Loading last save...");
      _progressService.Progress = LoadProgress();
      _logger.Log("Starting game...");
      StartGame();
    }

    private GameProgress LoadProgress() => _saveLoadService.LoadProgress();

    private void StartGame() =>
      _gameStateMachine.EnterState<LoadLevelState, string>(GetCurrentLevel());
    
    private string GetCurrentLevel() => _progressService.Progress.CurrentScene;
  }
}

