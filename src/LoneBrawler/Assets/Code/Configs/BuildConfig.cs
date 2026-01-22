// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

public enum BuildConfiguration
{
  Development,
  Shipping
}

[CreateAssetMenu(fileName = "BuildConfig", menuName = "Build/BuildConfig")]
public class GameBuildDAta : ScriptableObject
{
  public BuildConfiguration currentBuildConfiguration = BuildConfiguration.Development;
}

namespace Code.Configs
{
  public static class CurrentBuild
  {
    private static GameBuildDAta _buildConfig;

    public static BuildConfiguration GetConfiguration()
    {
      if (!_buildConfig)
      {
        _buildConfig = Resources.Load<GameBuildDAta>("Config/BuildConfig");

        if (!_buildConfig)
        {
          Debug.LogError("BuildConfig not found! Make sure it's in a Resources folder with correct path");
          _buildConfig = ScriptableObject.CreateInstance<GameBuildDAta>();
        }
      }
      return _buildConfig.currentBuildConfiguration;
    }
  }
}
