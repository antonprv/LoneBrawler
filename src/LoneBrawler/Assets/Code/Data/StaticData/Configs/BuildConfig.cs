// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs.BuildConfig;

using UnityEngine;

namespace Code.Data.StaticData.Configs
{

  [CreateAssetMenu(fileName = "BuildConfig", menuName = "StaticData/Config/BuildConfig")]
  public class GameBuildData : ScriptableObject
  {
    [NoNone]
    public BuildConfiguration currentBuildConfiguration = BuildConfiguration.Development;
  }
}
