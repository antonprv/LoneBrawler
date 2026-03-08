// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class SettingsWindow : WindowBase
  {
    public Slider soundSlider;
    public Slider musicSlider;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly ISaveLoadService _saveLoad;
    [Zenjex] private readonly IPersistentProgressService _progressService;
    [Zenjex] private readonly ISoundService _soundService;

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

    protected override void Cleanup() => base.Cleanup();
  }
}
