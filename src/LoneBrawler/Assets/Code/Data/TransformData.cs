// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data
{
  public sealed class TransformData
  {
    public Vector3Data Position;
    public QuatData Rotation;
    public Vector3Data Scale;

    public TransformData(Vector3Data position, QuatData rotation, Vector3Data scale)
    {
      Position = position;
      Rotation = rotation;
      Scale = scale;
    }

    public static TransformData Identity()
    {
      return new TransformData(
        Vector3.zero.AsVector3Data(),
        Quaternion.identity.AsQuatData(),
        Vector3.zero.AsVector3Data());
    }
  }
}
