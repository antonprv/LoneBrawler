using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;

using UnityEngine;

public class LogStatsCommand : IConsoleCommand
{
  private readonly IDevConsole _console;
  private int _logCount = 0;
  private int _warningCount = 0;
  private int _errorCount = 0;

  public string CommandName => "logstats";
  public string Description => "Show log statistics. Usage: logstats";

  public LogStatsCommand(IDevConsole console)
  {
    _console = console;
    Application.logMessageReceived += CountLogs;
  }

  private void CountLogs(string log, string trace, LogType type)
  {
    switch (type)
    {
      case LogType.Log:
        _logCount++;
        break;
      case LogType.Warning:
        _warningCount++;
        break;
      case LogType.Error:
      case LogType.Exception:
        _errorCount++;
        break;
    }
  }

  public void Execute(string[] args)
  {
    _console.AddMessage("=== Log Statistics ===", ConsoleMessageType.Log);
    _console.AddMessage($"Logs: {_logCount}", ConsoleMessageType.Log);
    _console.AddMessage($"Warnings: {_warningCount}", ConsoleMessageType.Warning);
    _console.AddMessage($"Errors: {_errorCount}", ConsoleMessageType.Error);
  }
}
