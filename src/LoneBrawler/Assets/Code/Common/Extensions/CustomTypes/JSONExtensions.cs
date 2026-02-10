// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes.Interfaces;

using UnityEngine;

namespace Code.Common.Extensions.CustomTypes
{
  public static class JSONExtensions
  {
    public static string ToSerialized(this object obj) =>
      JsonUtility.ToJson(obj);

    public static T ToDeserialized<T>(this string json) =>
      JsonUtility.FromJson<T>(json);

    public static bool IsValid<TData>(this TData data) where TData : class, IValidatableData =>
      data.IsDataNull();
  }
}
