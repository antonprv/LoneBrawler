// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace Code.Common.Extensions.Logging
{
  public interface IGameLog
  {
    void Log(string message);
    void Log(LogType logType, string message);
  }
}
