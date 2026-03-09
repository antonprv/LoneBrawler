// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine.UI;

using YG;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Shop
{
  public class WatchAdButton : ZenjexBehaviour
  {
    public Button addButton;

    [Zenjex] private readonly ISoulsTrackerService _soulsTrackerService;
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;

    protected override void OnAwake()
    {
      base.OnAwake();

      addButton.onClick.AddListener(HandleAddShow);
    }

    private void HandleAddShow()
    {
      var id = "souls";
      YG2.RewardedAdvShow(id, OnAddWatched);
    }

    private void OnAddWatched() =>
      _soulsTrackerService.AddSouls(_gameConfig.RewardedAddSouls);
  }
}
