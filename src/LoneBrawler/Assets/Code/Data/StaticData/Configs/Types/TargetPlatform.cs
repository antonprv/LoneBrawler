// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Data.StaticData.Configs.Types
{
  [Serializable]
  public enum TargetPlatform
  {
    None = 0,
    YandexGames = 1,
    RuStore = 2,
    GamePush = 3,
    ItchIoBrowser = 4,
    ItchIoDevice = 5
  }
}
