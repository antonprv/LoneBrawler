// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Configs.Types;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.Types;
using Code.UI.Windows.Types;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Attribute;

namespace Code.UI.Windows
{
  public class MainMenuWindow : WindowBase
  {
    public GameObject controlSelectWindow;

    public Button loadSave;
    public Button login;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly ISaveLoadService _saveLoad;
    [Zenjex] private readonly IBuildConfigSubservice _buildConfig;
    [Zenjex] private readonly IGameStateMachine _gameStateMachine;

    public override void Construct(
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(context, openButton);

    protected override void SetWindowType() =>
      windowTypeId = WindowTypeId.MainMenu;

    protected override void Initialize()
    {
      base.Initialize();

      CheckContext();
      CheckPlayerProgress();
    }

    private void CheckContext()
    {
      if (ConstructorContext != ConstructorContext.FromButton)
        closeWindow.gameObject.SetActive(false);
      else
        closeWindow.gameObject.SetActive(true);
    }

    protected override void SubscribeUpdates() => CheckPlayerProgress();

    protected override void Cleanup() => base.Cleanup();

    private void CheckPlayerProgress()
    {
      if (_gameStateMachine.GetCurrentStateType() != StateType.MainMenu)
      {
        DisableMainMenuButtons();
        return;
      }

      _logger.Log("Checking player progress...");

      var loadedProgress = _saveLoad.LoadProgress();

      if (loadedProgress.SaveTimeUTC == 0)
      {
        DisableMainMenuButtons();
        TryShowControlSelect();
      }
      else
      {
        _logger.Log($"Save found! SaveTimeUTC: {loadedProgress.SaveTimeUTC} - showing load save button");
        loadSave.gameObject.SetActive(true);
      }
    }

    private void DisableMainMenuButtons()
    {
      loadSave.gameObject.SetActive(false);
      login.gameObject.SetActive(false);
    }

    private void TryShowControlSelect()
    {
      if (_buildConfig.TargetPlatform == TargetPlatform.WebGL)
        controlSelectWindow.SetActive(true);
    }
  }
}
