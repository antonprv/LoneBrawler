// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Unity.Mathematics;

using UnityEngine;

namespace Code.Data.DataExtensions
{
  public static class QuatExtensions
  {

    public static QuatData AsQuatData(this Quaternion quat) =>
      new QuatData(quat.x, quat.y, quat.z, quat.w);

    public static Quaternion AsUnityQuat(this QuatData quatData) =>
      new Quaternion(quatData.X, quatData.Y, quatData.Z, quatData.W);

    public static bool IsNearlyEqual(
        this Quaternion a,
        Quaternion b,
        bool isPreNormalized = false,
        float epsilon = Constants.KINDA_SMALL_NUMBER
      )
    {
      if (!isPreNormalized)
      {
        a.Normalize();
        b.Normalize();
        float dot = math.dot(a, b);
        return 1f - dot <= epsilon;
      }

      float dotPreNormalized =
        math.dot(a.normalized, b.normalized);
      return 1f - dotPreNormalized <= epsilon;
    }

    public static bool IsNearlyZero(
      this Quaternion a,
      bool isPreNormalized = false,
      float epsilon = Constants.KINDA_SMALL_NUMBER
      )
    {
      return a.eulerAngles.sqrMagnitude <= epsilon;
    }
  }
}
