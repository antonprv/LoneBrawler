// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

public enum BuildConfiguration
{
  Development,
  Shipping
}

[CreateAssetMenu(fileName = "BuildConfig", menuName = "Config/BuildConfig")]
public class GameBuildData : ScriptableObject
{
  public BuildConfiguration currentBuildConfiguration = BuildConfiguration.Development;
}

namespace Code.Configs
{
  public static class CurrentBuild
  {
    private static GameBuildData _buildConfig;

    public static BuildConfiguration GetConfiguration()
    {
      if (!_buildConfig)
      {
        _buildConfig = Resources.Load<GameBuildData>("Config/BuildConfig");

        if (!_buildConfig)
        {
          Debug.LogError("BuildConfig not found! Make sure it's in a Resources folder with correct path");
          _buildConfig = ScriptableObject.CreateInstance<GameBuildData>();
        }
      }
      return _buildConfig.currentBuildConfiguration;
    }
  }
}
