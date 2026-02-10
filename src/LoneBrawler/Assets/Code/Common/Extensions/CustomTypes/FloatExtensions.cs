// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Unity.Mathematics;

namespace Code.Common.Extensions.CustomTypes
{
  public static class FloatExtensions
  {
    public static bool IsNearlyZero(
      this float f,
      float epsilon = Constants.KINDA_SMALL_NUMBER
      ) => f <= epsilon;

    public static bool IsNearlyEqual(
      this float f,
      float other,
      float epsilon = Constants.KINDA_SMALL_NUMBER
      ) => math.abs(f - other) <= epsilon;
  }
}
