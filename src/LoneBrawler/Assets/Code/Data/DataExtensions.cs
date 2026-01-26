// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Unity.Mathematics;

using UnityEngine;

namespace Code.Data
{
  public static class DataExtensions
  {
    public static Vector3Data AsVector3Data(this Vector3 vector3) =>
      new Vector3Data(vector3.x, vector3.y, vector3.z);

    public static Vector3 AsUnityVector(this Vector3Data vector3Data) =>
      new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);

    public static Vector3 AddY(this Vector3 vector, float Y)
    {
      vector.y += Y;
      return vector;
    }

    public static QuatData AsQuatData(this Quaternion quat) =>
      new QuatData(quat.x, quat.y, quat.z, quat.w);

    public static Quaternion AsUnityQuat(this QuatData quatData) =>
      new Quaternion(quatData.X, quatData.Y, quatData.Z, quatData.W);

    public static bool IsNearlyEqual(
        this Quaternion a,
        Quaternion b,
        float epsilon = Constants.KINDA_SMALL_NUMBER,
        bool isPreNormalized = false)
    {
      if (isPreNormalized == false)
      {
        float dotPreNormalized = math.dot(a.normalized, b.normalized);
        return 1f - math.abs(dotPreNormalized) <= epsilon;
      }

      float dot = math.dot(a, b);
      return 1f - math.abs(dot) <= epsilon;
    }

    public static TransformData AsTransformData(this Transform transform) =>
      new TransformData(
        transform.position.AsVector3Data(),
        transform.rotation.AsQuatData(),
        transform.localScale.AsVector3Data());

    public static void ApplyTo(this TransformData data, Transform unityTransform)
    {
      unityTransform.position = data.Position.AsUnityVector();
      unityTransform.rotation = data.Rotation.AsUnityQuat();
      unityTransform.localScale = data.Scale.AsUnityVector();
    }

    public static string ToSerialized(this object obj) =>
      JsonUtility.ToJson(obj);

    public static T ToDeserialized<T>(this string json) =>
      JsonUtility.FromJson<T>(json);

    public static bool IsValid<TData>(this TData data) where TData : class, IValidatableData =>
      data.IsDataNull();

    public static float GetLengthXZ(this Vector3 vector) =>
      Vector3.ProjectOnPlane(vector, Vector3.up).magnitude;
  }
}
