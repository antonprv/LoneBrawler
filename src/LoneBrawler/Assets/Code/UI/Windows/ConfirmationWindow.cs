// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.Time;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Attribute;

namespace Code.UI.Windows
{
  public class ConfirmationWindow : WindowBase
  {
    public CanvasGroup confirmGroup;

    public Button yesButton;

    public float smoothShowSpeed = 0.5f;

    [Zenjex] private readonly ITimeService _timeService;

    private readonly float _canvasOnAlpha = 1f;
    private readonly float _canvasOffAlpha = 0f;

    private void OnEnable() => InterpCanvas(_canvasOnAlpha, OnAppear);

    protected override void SetWindowType() =>
      windowTypeId = WindowTypeId.ConfirmScreen;

    protected override void OnCloseButtonClicked()
    {
      DeactivateButtons();
      InterpCanvas(_canvasOffAlpha, OnDisappear);
    }

    protected override void Cleanup()
    {
      base.Cleanup();
      closeWindow.onClick.RemoveAllListeners();
      yesButton.onClick.RemoveAllListeners();
    }

    private void ActivateButtons()
    {
      closeWindow.interactable = true;
      yesButton.interactable = true;
    }

    private void DeactivateButtons()
    {
      closeWindow.interactable = false;
      yesButton.interactable = false;
    }

    private void InterpCanvas(float canvasAlpha, Action onComplete = null)
    {
      LeanTween
        .alphaCanvas(
        confirmGroup,
        canvasAlpha,
        smoothShowSpeed * _timeService.DeltaAt100FPS
        ).setOnComplete(onComplete);
    }

    private void OnAppear() => ActivateButtons();

    private void OnDisappear() => gameObject.SetActive(false);
  }
}
