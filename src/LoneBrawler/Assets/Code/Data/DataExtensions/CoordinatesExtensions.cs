// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions.Types;
using Code.Data.SaveData.Common;

using UnityEngine;

namespace Code.Data.DataExtensions
{
  public static class CoordinatesExtensions
  {
    public static void ApplyTo(this Coordinates coords, Transform unityTransform)
    {
      unityTransform.SetPositionAndRotation(coords.Position, coords.Rotation);
      unityTransform.localScale = Vector3.one;
    }

    public static TransformData AsTransformData(this Coordinates coords) =>
      new TransformData(
        coords.Position.AsVector3Data(),
        coords.Rotation.AsQuatData(),
        Vector3.one.AsVector3Data()
        );
  }
}
