// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole.Interfaces;

namespace Code.Infrastructure.Services.DevConsole
{
  public interface IDevConsole
  {
    public bool IsEnabled { get; }
    public void Toggle();
    public void ExecuteCommand(string command);
    public void RegisterCommand(IConsoleCommand command);
    public void AddMessage(string message, ConsoleMessageType type = ConsoleMessageType.Log);
    public string[] GetMessages();
    public void ClearMessages();
    public void SetCaptureUnityLogs(bool capture);
  }

  public enum ConsoleMessageType
  {
    Log,
    Warning,
    Error,
    Command,
    Success,
    UnityLog  // For captured Unity Debug.Log messages
  }
}
