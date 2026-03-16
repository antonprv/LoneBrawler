// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Data.StaticData.Types.Buff
{
  [System.Serializable]
  public enum BuffActivationType
  {
    None = 0,
    Burst = 1,
    Constant = 2,
    Duration = 3
  }

  public static class LocalisationExtensions
  {
    public static string GetRussianName(this BuffActivationType type)
    {
      switch (type)
      {
        case BuffActivationType.None:
          break;
        case BuffActivationType.Burst:
          return "Мгновенный";
        case BuffActivationType.Constant:
          return "Постоянный";
        case BuffActivationType.Duration:
          return "Временный";
        default:
          break;
      }
      return null;
    }
  }
}
