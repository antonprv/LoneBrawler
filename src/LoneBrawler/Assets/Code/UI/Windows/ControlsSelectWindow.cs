// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.SaveData.Types;
using Code.Infrastructure.Services.Time;
using Code.UI.Services.PlatformControls.Interfaces;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Windows
{
  public class ControlsSelectWindow : ZenjexBehaviour
  {
    public Button mobileControlsButton;
    public Button pcControlsButton;

    public CanvasGroup controlSelectGroup;

    public float smoothShowSpeed = 0.25f;

    private readonly float _canvasOffAlpha = 0f;
    private readonly float _canvasOnAlpha = 1f;

    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly IPlatformControls _platformControls;

    protected override void OnAwake()
    {
      base.OnAwake();

      controlSelectGroup.alpha = _canvasOffAlpha;

      SubscribeUpdates();
    }

    private void SubscribeUpdates()
    {
      if (mobileControlsButton != null)
        mobileControlsButton.onClick.AddListener(ToggleMobileControls);

      if (pcControlsButton != null)
        pcControlsButton.onClick.AddListener(TogglePCControls);
    }

    private void TogglePCControls() =>
      ToggleControls(ControlScheme.PC);

    private void ToggleMobileControls() =>
      ToggleControls(ControlScheme.Mobile);

    private void OnEnable() => InterpCanvas(_canvasOnAlpha);

    private void OnDisable()
    {
      if (mobileControlsButton != null)
        mobileControlsButton.onClick.RemoveAllListeners();

      if (pcControlsButton != null)
        pcControlsButton.onClick.RemoveAllListeners();
    }

    private void ToggleControls(ControlScheme controls)
    {
      DeactivateButtons();
      _platformControls.SetScheme(controls);
      InterpCanvas(_canvasOffAlpha, OnDisappear);
    }

    private void OnDisappear() => gameObject.SetActive(false);

    private void InterpCanvas(float canvasAlpha, Action onComplete = null)
    {
      LeanTween
        .alphaCanvas(
        controlSelectGroup,
        canvasAlpha,
        smoothShowSpeed * _timeService.DeltaAt100FPS
        );
    }

    private void DeactivateButtons()
    {
      mobileControlsButton.interactable = false;
      pcControlsButton.interactable = false;
      controlSelectGroup.interactable = false;
      controlSelectGroup.blocksRaycasts = false;
    }
  }
}
