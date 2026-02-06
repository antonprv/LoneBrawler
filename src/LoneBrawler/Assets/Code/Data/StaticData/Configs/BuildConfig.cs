// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs.Types;

using UnityEngine;

namespace Code.Data.StaticData.Configs
{

  [CreateAssetMenu(fileName = "BuildConfig", menuName = "StaticData/Config/BuildConfig")]
  public class GameBuildData : ScriptableObject
  {
    [NoNone]
    public BuildConfiguration BuildConfiguration = BuildConfiguration.Development;

    [NoNone]
    public TargetPlatform Platform = TargetPlatform.YandexGames;
  }
}
