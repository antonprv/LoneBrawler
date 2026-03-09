// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.Time;
using Code.UI.Windows.Types;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Attribute;

namespace Code.UI.Windows
{
  public class SettingsWindow : WindowBase
  {
    public float smoothShowSpeed = 1.25f;

    public CanvasGroup settingsGroup;

    public Slider soundSlider;
    public Slider musicSlider;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly ISaveLoadService _saveLoad;
    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly ISoundService _soundService;

    private const float _canvasOnAlpha = 1f;
    private const float _canvasOffAlpha = 0f;

    protected override void OnAwake()
    {
      base.OnAwake();
      Construct(ConstructorContext.FromButton, null);
    }

    public override void Construct(
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(context, openButton);

    protected override void SetWindowType() =>
      windowTypeId = WindowTypeId.Settings;

    protected override void Initialize()
    {
      base.Initialize();

      LoadCurrentSettings();
      InitializeService();
      SyncSlidersToService();
    }

    private void OnEnable() => InterpCanvas(_canvasOnAlpha);

    private void InterpCanvas(float canvasAlpha, Action onComplete = null)
    {
      LeanTween
        .alphaCanvas(
        settingsGroup,
        canvasAlpha,
        smoothShowSpeed * _timeService.DeltaAt100FPS
        ).setOnComplete(onComplete);
    }

    private void LoadCurrentSettings() =>
      _progressService.SystemSettings = _saveLoad.LoadSettings();

    private void InitializeService() =>
      _soundService.ReadSettings(_progressService.SystemSettings);

    protected override void SubscribeUpdates()
    {
      soundSlider.onValueChanged.AddListener(HandleSoundVolumeChanged);
      musicSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
    }

    private void SyncSlidersToService()
    {
      soundSlider.SetValueWithoutNotify(_soundService.SoundVolumeRP.CurrentValue);
      musicSlider.SetValueWithoutNotify(_soundService.MusicVolumeRP.CurrentValue);
    }

    private void HandleMusicVolumeChanged(float volume)
    {
      _soundService.MusicVolumeRP.Value = volume;
      _logger.LogValue(_soundService.MusicVolumeRP, volume);
    }

    private void HandleSoundVolumeChanged(float volume)
    {
      _soundService.SoundVolumeRP.Value = volume;
      _logger.LogValue(_soundService.SoundVolumeRP, volume);
    }

    protected override void OnCloseButtonClicked()
    {
      _soundService.WriteToSettings(_progressService.SystemSettings);
      _saveLoad.SaveProgress();

      InterpCanvas(_canvasOffAlpha, OnDisappear);
    }

    private void OnDisappear() => gameObject.SetActive(false);
  }
}
