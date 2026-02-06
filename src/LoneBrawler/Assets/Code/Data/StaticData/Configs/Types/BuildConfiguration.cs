// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Data.StaticData.Configs.Types
{
  [Serializable]
  public enum BuildConfiguration
  {
    None = 0,
    Development = 1,
    Shipping = 2
  }
}
