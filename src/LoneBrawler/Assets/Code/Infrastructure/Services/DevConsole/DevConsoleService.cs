// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Linq;

using Code.Infrastructure.Services.DevConsole.Commands;
using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.DevConsole
{
  public class DevConsoleService : IDevConsole
  {
    private readonly Dictionary<string, IConsoleCommand> _commands;
    private readonly List<ConsoleMessage> _messages;
    private readonly IBuildConfigSubservice _buildConfig;
    private const int _maxMessages = 500;
    private bool _captureUnityLogs;

    public bool IsEnabled { get; private set; }

    public DevConsoleService(IBuildConfigSubservice buildConfig)
    {
      _buildConfig = buildConfig;
      _commands = new Dictionary<string, IConsoleCommand>();
      _messages = new List<ConsoleMessage>();
      IsEnabled = false;
      _captureUnityLogs = true;

      // Register help command
      if (CanUseConsole())
      {
        RegisterCommand(new HelpCommand(this, _commands));
        AddMessage("Developer Console initialized. Type 'help' for commands.", ConsoleMessageType.Log);

        // Subscribe to Unity logs
        SubscribeToUnityLogs();
      }
    }

    ~DevConsoleService()
    {
      UnsubscribeFromUnityLogs();
    }

    public void RegisterCommand(IConsoleCommand command)
    {
      if (!CanUseConsole())
        return;

      string commandName = command.CommandName.ToLower();

      if (_commands.ContainsKey(commandName))
      {
        AddMessage($"Command '{commandName}' is already registered!", ConsoleMessageType.Warning);
        return;
      }

      _commands.Add(commandName, command);
      // Don't log command registration to avoid spam
    }

    public void Toggle()
    {
      if (!CanUseConsole())
      {
        AddMessage("Console is disabled in Shipping builds", ConsoleMessageType.Warning);
        return;
      }

      IsEnabled = !IsEnabled;
    }

    public void ExecuteCommand(string commandLine)
    {
      if (!CanUseConsole())
        return;

      if (string.IsNullOrWhiteSpace(commandLine))
        return;

      // Add command to history
      AddMessage($"> {commandLine}", ConsoleMessageType.Command);

      string[] parts = commandLine.Split(' ');
      string commandName = parts[0].ToLower();
      string[] args = parts.Skip(1).ToArray();

      if (_commands.TryGetValue(commandName, out IConsoleCommand command))
      {
        command.Execute(args);
      }
      else
      {
        AddMessage($"Unknown command: {commandName}", ConsoleMessageType.Error);
        AddMessage($"Type 'help' to see available commands", ConsoleMessageType.Log);
      }
    }

    public void AddMessage(string message, ConsoleMessageType type = ConsoleMessageType.Log)
    {
      _messages.Add(new ConsoleMessage(message, type));

      // Limit message history
      if (_messages.Count > _maxMessages)
      {
        _messages.RemoveRange(0, _messages.Count - _maxMessages);
      }
    }

    public string[] GetMessages()
    {
      return _messages.Select(m => m.FormattedMessage).ToArray();
    }

    public void ClearMessages()
    {
      _messages.Clear();
      AddMessage("Console cleared", ConsoleMessageType.Log);
    }

    public void SetCaptureUnityLogs(bool capture)
    {
      if (_captureUnityLogs == capture)
        return;

      _captureUnityLogs = capture;

      if (capture)
        SubscribeToUnityLogs();
      else
        UnsubscribeFromUnityLogs();

      AddMessage($"Unity log capture {(capture ? "enabled" : "disabled")}", ConsoleMessageType.Log);
    }

    private void SubscribeToUnityLogs()
    {
      Application.logMessageReceived += HandleUnityLog;
    }

    private void UnsubscribeFromUnityLogs()
    {
      Application.logMessageReceived -= HandleUnityLog;
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
      if (!_captureUnityLogs)
        return;

      // Don't capture our own console messages to avoid recursion
      if (logString.StartsWith("[Console]"))
        return;

      ConsoleMessageType messageType = type switch
      {
        LogType.Error => ConsoleMessageType.Error,
        LogType.Assert => ConsoleMessageType.Error,
        LogType.Warning => ConsoleMessageType.Warning,
        LogType.Exception => ConsoleMessageType.Error,
        _ => ConsoleMessageType.UnityLog
      };

      // Format message with timestamp
      string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
      string formattedMessage = $"[{timestamp}] {logString}";

      // Add stack trace for errors and exceptions (collapsed)
      if (type == LogType.Error || type == LogType.Exception)
      {
        if (!string.IsNullOrEmpty(stackTrace))
        {
          // Only add first line of stack trace to save space
          string firstLine = stackTrace.Split('\n')[0];
          formattedMessage += $"\n  → {firstLine}";
        }
      }

      AddMessage(formattedMessage, messageType);
    }

    private bool CanUseConsole()
    {
      return _buildConfig.IsDevelopment();
    }
  }

  public class ConsoleMessage
  {
    public string Message { get; }
    public ConsoleMessageType Type { get; }
    public string FormattedMessage { get; }

    public ConsoleMessage(string message, ConsoleMessageType type)
    {
      Message = message;
      Type = type;
      FormattedMessage = FormatMessage(message, type);
    }

    private string FormatMessage(string message, ConsoleMessageType type)
    {
      string prefix = type switch
      {
        ConsoleMessageType.Warning => "<color=yellow>[WARNING]</color> ",
        ConsoleMessageType.Error => "<color=red>[ERROR]</color> ",
        ConsoleMessageType.Command => "<color=cyan>",
        ConsoleMessageType.Success => "<color=green>[OK]</color> ",
        ConsoleMessageType.UnityLog => "<color=white>[Unity]</color> ",
        _ => ""
      };

      string suffix = type == ConsoleMessageType.Command ? "</color>" : "";

      return prefix + message + suffix;
    }
  }
}
