// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
namespace YG.Insides
{
  public partial class ProjectSettings
  {
    public bool skipInterAdvAfterReward;

    [ApplySettings]
    private void RewardAdv_ApplySettings()
    {
      if (YG2.infoYG.platformToggles.skipInterAdvAfterReward)
        YG2.infoYG.RewardedAdv.skipInterAdvAfterReward = skipInterAdvAfterReward;
    }
  }

  public partial class PlatformToggles
  {
    public bool skipInterAdvAfterReward;
  }
}
#endif