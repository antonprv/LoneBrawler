// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Runtime.CompilerServices;

namespace Code.Common.Domain.FastMathUtils
{
  /// <summary>
  /// Extension methods for convenient usage
  /// </summary>
  public static class FastMathExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FastSqrt(this float value) =>
      FastMath.FastSqrt(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FastInvSqrt(this float value) =>
      FastMath.FastInvSqrt(value);
  }
}
