// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs.Types;

namespace Code.Data.StaticData.Configs
{

  [UnityEngine.CreateAssetMenu(fileName = "BuildConfig", menuName = "StaticData/Config/BuildConfig")]
  public class GameBuildData : UnityEngine.ScriptableObject
  {
    [FilteredEnum(BuildConfiguration.None)]
    public BuildConfiguration BuildConfiguration = BuildConfiguration.Development;

    [FilteredEnum(TargetPlatform.None)]
    public TargetPlatform Platform = TargetPlatform.WebGL;
  }
}
