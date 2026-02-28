// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Tests.PlayMode.Helpers
{
  internal static class ScriptableObjectExtensions
  {
    public static T Also<T>(this T obj, System.Action<T> action)
    {
      action(obj);
      return obj;
    }
  }
}
