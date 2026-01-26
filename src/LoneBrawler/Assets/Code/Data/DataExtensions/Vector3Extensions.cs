// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.DataExtensions
{
  public static class Vector3Extensions
  {
    public static Vector3 AddY(this Vector3 vector, float Y)
    {
      vector.y += Y;
      return vector;
    }

    public static bool IsNearlyZero(
      this Vector3 a,
      float epsilon = Constants.KINDA_SMALL_NUMBER
      )
    {
      return a.sqrMagnitude <= epsilon;
    }

    public static float GetLengthXZ(this Vector3 vector) =>
      Vector3.ProjectOnPlane(vector, Vector3.up).magnitude;

    public static Vector3 AsUnityVector(this Vector3Data vector3Data) =>
      new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);
    public static Vector3Data AsVector3Data(this Vector3 vector3) =>
      new Vector3Data(vector3.x, vector3.y, vector3.z);
  }
}
