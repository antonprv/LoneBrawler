// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Diagnostics;
using System.Reflection;

using U = UnityEngine;

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

      if (callerType != null && callingMethod != null)
      {
        U.Debug.Log(
          $"Log [{callerType.Name}.{callingMethod.Name}] {message}");
      }
      else
      {
        U.Debug.LogWarning(
          $"{nameof(IGameLog)}: Unable to determine the caller's information for logging.");
      }
#endif
    }

    public void Log(U.LogType logType, string message)
    {
#if UNITY_EDITOR
      StackFrame frame = new StackFrame(1);
      MethodBase callingMethod = frame.GetMethod();
      Type callerType = callingMethod?.DeclaringType;

      if (callerType != null && callingMethod != null)
      {
        U.Debug.unityLogger.Log(
          logType, $"Log [{callerType.Name}.{callingMethod.Name}] {message}");
      }
      else
      {
        U.Debug.LogWarning(
          $"{nameof(IGameLog)}: Unable to determine the caller's information for logging.");
      }
#endif
    }
  }
}
