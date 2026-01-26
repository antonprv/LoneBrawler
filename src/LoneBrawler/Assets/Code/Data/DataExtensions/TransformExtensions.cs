// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.DataExtensions
{
  internal static class TransformExtensions
  {

    public static void ApplyTo(this TransformData data, Transform unityTransform)
    {
      unityTransform.position = data.Position.AsUnityVector();
      unityTransform.rotation = data.Rotation.AsUnityQuat();
      unityTransform.localScale = data.Scale.AsUnityVector();
    }
    public static TransformData AsTransformData(this Transform transform) =>
      new TransformData(
        transform.position.AsVector3Data(),
        transform.rotation.AsQuatData(),
        transform.localScale.AsVector3Data());
  }
}
