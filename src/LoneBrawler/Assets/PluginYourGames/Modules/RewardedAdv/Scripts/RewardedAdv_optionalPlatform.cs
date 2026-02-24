// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace YG.Insides
{
  public partial class OptionalPlatform
  {
    public Action onLoadedRewardedAdv;
    public Action onClickedRewardedAdv;

    public void LoadRewardedAdv()
    {
#if !UNITY_EDITOR
            YG2.iPlatform.LoadRewardedAdv();
#endif
    }
  }
}