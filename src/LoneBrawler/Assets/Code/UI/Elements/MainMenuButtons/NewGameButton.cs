// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Gameplay.Audio.Sound.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.RestartGame.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.Types;
using Code.UI.Windows;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.MainMenuButtons
{
  public class NewGameButton : ZenjexBehaviour
  {
    public Button button;

    public ConfirmationWindow confirmScreen;

    private IButtonSound _sound;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly IStaticDataService _staticData;
    [Zenjex] private readonly ISaveLoadService _saveLoadService;
    [Zenjex] private readonly IGameStateMachine _gameStateMachine;
    [Zenjex] private readonly IRestartGameService _restartGame;

    protected override void OnAwake()
    {
      _sound = GetComponentInChildren<IButtonSound>();

      if (_sound == null)
        button.onClick.AddListener(CheckProgress);
      else
        _sound.OnClickSoundFinished
          .Subscribe(_ => CheckProgress())
          .AddTo(this.GetCancellationTokenOnDestroy());
    }

    private void CheckProgress()
    {
      if (_progressService.Progress.SaveTimeUTC == 0)
      {
        ResetGame(); // If there's no progress - reset the game
        return;
      }

      // If there is - show confirm screen
      if (confirmScreen == null) return;

      confirmScreen.yesButton.onClick.RemoveAllListeners(); // duplicate calls protection
      confirmScreen.yesButton.onClick.AddListener(ResetGame);
      confirmScreen.gameObject.SetActive(true);
    }

    private void ResetGame()
    {
      if (_gameStateMachine.GetCurrentState() == StateType.MainMenu)
      {
        ResetFromMainMenu();
        return;
      }

      _restartGame.RequestRestart();
    }

    private void ResetFromMainMenu()
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
      new(_staticData.PlayerData, _staticData.InventoryConfig, SceneAddresses.MainSceneAddress);

    private void SaveNewProgress() => _saveLoadService.SaveProgress();

    private void StartGame() =>
      _gameStateMachine.EnterState<LoadLevelState, string>(GetCurrentLevel());

    private string GetCurrentLevel() => _progressService.Progress.CurrentScene;
  }
}
