// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Commands.Performance;
using Code.Infrastructure.Services.DevConsole.Types;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole.Commands
{
  [TestFixture]
  public class SetFPSCommandTests
  {
    private IDevConsole _console;
    private SetFPSCommand _command;

    [SetUp]
    public void SetUp()
    {
      _console = Substitute.For<IDevConsole>();
      _command = new SetFPSCommand(_console);
    }

    [Test]
    public void CommandName_IsSetFps()
    {
      Assert.That(_command.CommandName, Is.EqualTo("set_fps"));
    }

    [Test]
    public void Execute_NoArgs_PrintsUsage()
    {
      _command.Execute(new string[0]);
      _console.Received().AddMessage(Arg.Any<string>(), ConsoleMessageType.Warning);
    }

    [Test]
    public void Execute_ValidNumber_AddsSuccessMessage()
    {
      _command.Execute(new[] { "60" });
      _console.Received().AddMessage(Arg.Is<string>(s => s.Contains("60")), ConsoleMessageType.Success);
    }

    [Test]
    public void Execute_InvalidNumber_AddsErrorMessage()
    {
      _command.Execute(new[] { "notanumber" });
      _console.Received().AddMessage(Arg.Any<string>(), ConsoleMessageType.Error);
    }

    [Test]
    public void Execute_NegativeNumber_AcceptsValue()
    {
      // UnityEngine.Application.targetFrameRate = -1 means "unlimited"
      _command.Execute(new[] { "-1" });
      _console.Received().AddMessage(Arg.Is<string>(s => s.Contains("-1")), ConsoleMessageType.Success);
    }
  }
}
