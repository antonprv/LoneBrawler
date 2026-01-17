// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Code.Common.Extensions.Logging
{
  public class GameLogger : IGameLog
  {
    public void Log(string message)
    {
#if UNITY_EDITOR
      StackFrame frame = new StackFrame(1);
      MethodBase callingMethod = frame.GetMethod();
      Type callerType = callingMethod?.DeclaringType;

      if (callerType != null)
      {

        UnityEngine.Debug.Log($"Log {callerType.Name}: {message}");

      }
      else
      {

        UnityEngine.Debug.LogWarning("Unable to determine the caller's type for logging.");
      }
#endif
    }
  }
}
