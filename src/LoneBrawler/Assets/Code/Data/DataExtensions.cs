// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data
{
  public static class DataExtensions
  {
    public static Vector3Data AsVector3Data(this Vector3 vector3) =>
      new Vector3Data(vector3.x, vector3.y, vector3.z);

    public static Vector3 AsUnityVector(this Vector3Data vector3Data) =>
      new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);

    public static QuatData AsQuatData(this Quaternion quat) =>
      new QuatData(quat.x, quat.y, quat.z, quat.w);

    public static Quaternion AsUnityQuat(this QuatData quatData) =>
      new Quaternion(quatData.X, quatData.Y, quatData.Z, quatData.W);

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
  }
}
