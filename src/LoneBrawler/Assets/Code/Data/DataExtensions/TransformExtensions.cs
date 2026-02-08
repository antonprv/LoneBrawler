// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions.Types;
using Code.Data.SaveData.Common;

using UnityEngine;

namespace Code.Data.DataExtensions
{
  public static class TransformExtensions
  {

    public static void ApplyTo(this TransformData data, Transform unityTransform)
    {
      unityTransform.SetPositionAndRotation(
        data.Position.AsUnityVector(),
        data.Rotation.AsUnityQuat()
        );
      unityTransform.localScale = data.Scale.AsUnityVector();
    }

    public static TransformData AsTransformData(this Transform transform) =>
      new TransformData(
        transform.position.AsVector3Data(),
        transform.rotation.AsQuatData(),
        transform.localScale.AsVector3Data());


    public static Coordinates AsCoordinates(this TransformData transform) =>
      new Coordinates(
        transform.Position.AsUnityVector(),
        transform.Rotation.AsUnityQuat());

    public static Coordinates AsCoordinates(this Transform transform) =>
      new Coordinates(transform.position, transform.rotation);

  }
}
