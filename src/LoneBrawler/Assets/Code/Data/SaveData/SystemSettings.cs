// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Types;

namespace Code.Data.SaveData
{
  [System.Serializable]
  public class SystemSettings
  {
    public float SoundVolume;
    public float MusicVolume;

    public ControlScheme Controls;

    public SystemSettings()
    {
      SoundVolume = 1.0f;
      MusicVolume = 1.0f;

      Controls = ControlScheme.None;
    }

    public SystemSettings(float soundVolume, float musicVolume)
    {
      SoundVolume = soundVolume;
      MusicVolume = musicVolume;

      Controls = ControlScheme.None;
    }
  }
}
