// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using R3;

namespace Code.Infrastructure.Services.SoundService.Interfaces
{
  public interface ISoundService : ISettingsReader, ISettingsWriter
  {
    ReactiveProperty<float> MusicVolumeRP { get; set; }
    ReactiveProperty<float> SoundVolumeRP { get; set; }
  }
}
