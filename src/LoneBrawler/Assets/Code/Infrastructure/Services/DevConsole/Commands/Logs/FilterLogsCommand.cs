// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;

using UnityEngine;

public class FilterLogsCommand : IConsoleCommand
{
  private readonly IDevConsole _console;
  //private LogType _filter = LogType.Log;

  public string CommandName => "filter";
  public string Description => "Filter logs by type. Usage: filter <log|warning|error|all>";

  public FilterLogsCommand(IDevConsole console)
  {
    _console = console;
  }

  public void Execute(string[] args)
  {
    if (args.Length < 1)
    {
      _console.AddMessage(Description, ConsoleMessageType.Warning);
      return;
    }

    // TODO: Implementation
    _console.AddMessage($"Filter set to: {args[0]}", ConsoleMessageType.Success);
  }
}
