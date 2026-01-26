// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Data.DataExtensions
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
