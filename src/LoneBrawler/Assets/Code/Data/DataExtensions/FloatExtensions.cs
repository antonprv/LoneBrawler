// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Data.DataExtensions
{
  public static class FloatExtensions
  {
    public static bool IsNearlyZero(
      this float f,
      float epsilon = Constants.KINDA_SMALL_NUMBER
      ) => f <= epsilon;
  }
}
