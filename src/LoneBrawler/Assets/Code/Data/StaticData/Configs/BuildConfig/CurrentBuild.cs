// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.StaticData.Configs.BuildConfig
{
  public static class CurrentBuild
  {
    private static GameBuildData _buildConfig;

    public static BuildConfiguration GetConfiguration()
    {
      if (!_buildConfig)
      {
        _buildConfig = Resources.Load<GameBuildData>("StaticData/Config/BuildConfig");

        if (!_buildConfig)
        {
          Debug.LogError($"{typeof(GameBuildData)} not found!" +
            $" Make sure it's in a Resources folder with correct path");
          return BuildConfiguration.None;
        }
      }
      return _buildConfig.currentBuildConfiguration;
    }
  }
}
