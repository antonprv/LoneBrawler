// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Runtime.InteropServices;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;
using UnityEngine.UI;

using YG;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Shop
{
  public class WatchAdButton : ZenjexBehaviour
  {
    public Button addButton;

    public string debugText = "TEST: add watched";

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly ISoulsTrackerService _soulsTrackerService;
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;
    [Zenjex] private readonly IBuildConfigSubservice _buildConfig;

    [DllImport("__Internal")]
    private static extern void ShowAlertMessage(string message);

    protected override void OnAwake()
    {
      base.OnAwake();

      addButton.onClick.AddListener(HandleAddShow);
    }

    private void HandleAddShow()
    {
      _logger.Log(debugText);

      if (_buildConfig.UseAddSdk == true)
      {
        var id = "souls";
        YG2.RewardedAdvShow(id, OnAddWatched);
      }
      else
      {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
          ShowAlertMessage(debugText);

        OnAddWatched();
      }
    }

    private void OnAddWatched() =>
      _soulsTrackerService.AddSouls(_gameConfig.RewardedAddSouls);
  }
}
