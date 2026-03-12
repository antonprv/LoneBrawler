// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.DevConsole.Commands.Logs;
using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.DevConsole.Types;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole.Commands
{
  [TestFixture]
  public class ToggleUnityLogsCommandTests
  {
    private IDevConsole _console;
    private ToggleUnityLogsCommand _command;

    [SetUp]
    public void SetUp()
    {
      _console = Substitute.For<IDevConsole>();
      _command = new ToggleUnityLogsCommand(_console);
    }

    [Test]
    public void CommandName_IsToggleUnityLogs()
    {
      Assert.That(_command.CommandName, Is.EqualTo("toggle_unity_logs"));
    }

    [Test]
    public void Execute_FirstCall_DisablesCapture()
    {
      // Default _isEnabled = true → toggle → false
      _command.Execute(new string[0]);
      _console.Received(1).SetCaptureUnityLogs(false);
    }

    [Test]
    public void Execute_SecondCall_ReEnablesCapture()
    {
      _command.Execute(new string[0]); // → false
      _command.Execute(new string[0]); // → true
      _console.Received(1).SetCaptureUnityLogs(true);
    }

    [Test]
    public void Execute_AddsSuccessMessage()
    {
      _command.Execute(new string[0]);
      _console.Received().AddMessage(Arg.Any<string>(), ConsoleMessageType.Success);
    }
  }
}
