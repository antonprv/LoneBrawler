// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.SoundService.Interfaces;

using R3;

namespace Code.Infrastructure.Services.SoundService
{
  public class SoundService : ISoundService
  {
    public ReactiveProperty<float> SoundVolumeRP { get; set; } = new ReactiveProperty<float>(1f);
    public ReactiveProperty<float> MusicVolumeRP { get; set; } = new ReactiveProperty<float>(1f);

    public void ReadSettings(SystemSettings systemSettings)
    {
      SoundVolumeRP.Value = systemSettings.SoundVolume;
      MusicVolumeRP.Value = systemSettings.MusicVolume;
    }

    public void WriteToSettings(SystemSettings systemSettings)
    {
      systemSettings.SoundVolume = SoundVolumeRP.CurrentValue;
      systemSettings.MusicVolume = MusicVolumeRP.CurrentValue;
    }
  }
}
