// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data
{
  public static class DataExtensions
  {
    public static Vector3Data AsVector3Data(this Vector3 vector3) =>
      new Vector3Data(vector3.x, vector3.y, vector3.z);

    public static QuatData AsQuatData(this Quaternion quat) =>
      new QuatData(quat.x, quat.w, quat.z, quat.w);

    public static TransformData AsTransformData(this Transform transform) =>
      new TransformData(
        transform.position.AsVector3Data(),
        transform.rotation.AsQuatData(),
        transform.localScale.AsVector3Data());
  }
}
