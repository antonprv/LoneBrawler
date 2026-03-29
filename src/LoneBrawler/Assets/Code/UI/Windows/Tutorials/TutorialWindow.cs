// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.SaveData.Tutorials.Types;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.Time;

using UnityEngine;

using Zenjex.Extensions.Attribute;

namespace Code.UI.Windows.Tutorials
{
  public class TutorialWindow : WindowBase
  {
    public CanvasGroup tutorialGroup;
    public float smoothShowSpeed = 0.25f;

    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly IInputService _inputService;

    private const float canvasOffAlpha = 0f;

    protected override void Initialize()
    {
      base.Initialize();
      DisableInput();
      ShowIfNotWatched();
    }

    protected override void SetWindowType() => windowTypeId = WindowTypeId.Tutorial;

    protected override void OnCloseButtonClicked()
    {
      closeWindow.interactable = false;
      InterpCanvas(canvasOffAlpha, () => Destroy(gameObject));
    }

    protected override void Cleanup()
    {
      base.Cleanup();
      EnableInput();
      MarkWatched();
    }

    private void ShowIfNotWatched()
    {
      if (_progressService.Progress.WatchedTutorials.Tutorials.Contains(TutorialType.Controls))
        gameObject.SetActive(false);
    }


    private void DisableInput() => _inputService.GameInputEnabled = false;

    private void EnableInput() => _inputService.GameInputEnabled = true;

    private void InterpCanvas(float canvasAlpha, Action onComplete = null)
    {
      LeanTween
        .alphaCanvas(
        tutorialGroup,
        canvasAlpha,
        smoothShowSpeed * _timeService.DeltaAt100FPS
        ).setOnComplete(onComplete);
    }

    private void MarkWatched()
    {
      _progressService
        .Progress
        .WatchedTutorials
        .Tutorials
        .Add(TutorialType.Controls);
    }
  }
}
