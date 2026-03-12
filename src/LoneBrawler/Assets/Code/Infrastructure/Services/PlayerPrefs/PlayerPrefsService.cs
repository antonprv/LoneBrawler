// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UPrefs = UnityEngine.PlayerPrefs;
using RDPrefs = RedefineYG.PlayerPrefs;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Code.Infrastructure.Services.PlayerPrefs.Interfaces;

namespace Code.Infrastructure.Services.PlayerPrefs
{
  public class PlayerPrefsService : IPlayerPrefsService
  {
    private readonly IBuildConfigSubservice _buildConfig;

    public PlayerPrefsService(IBuildConfigSubservice buildConfig)
    {
      _buildConfig = buildConfig;
    }

    public void DeleteAll()
    {
      if (_buildConfig.UseCloudSave)
        RDPrefs.DeleteAll();
      else
        UPrefs.DeleteAll();
    }

    public void Save()
    {
      if (_buildConfig.UseCloudSave)
        RDPrefs.Save();
      else
        UPrefs.Save();
    }

    public string GetString(string payload)
    {
      if (_buildConfig.UseCloudSave)
        return RDPrefs.GetString(payload);
      return UPrefs.GetString(payload);
    }

    public void SetString(string key, string value)
    {
      if (_buildConfig.UseCloudSave)
        RDPrefs.SetString(key, value);
      UPrefs.SetString(key, value);
    }

    public void DeleteKey(string key)
    {
      if (_buildConfig.UseCloudSave)
        RDPrefs.DeleteKey(key);
      else
        UPrefs.DeleteKey(key);
    }
  }


}
