// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Commands.Logs;
using Code.Infrastructure.Services.DevConsole.Types;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole.Commands
{
  [TestFixture]
  public class FilterLogsCommandTests
  {
    private IDevConsole _console;
    private FilterLogsCommand _command;

    [SetUp]
    public void SetUp()
    {
      _console = Substitute.For<IDevConsole>();
      _command = new FilterLogsCommand(_console);
    }

    [Test]
    public void CommandName_IsFilter()
    {
      Assert.That(_command.CommandName, Is.EqualTo("filter"));
    }

    [Test]
    public void Execute_NoArgs_PrintsCurrentFilter()
    {
      _console.GetLogFilter().Returns(ConsoleMessageType.All);
      _command.Execute(new string[0]);
      _console.Received().AddMessage(Arg.Is<string>(s => s.Contains("filter")), Arg.Any<ConsoleMessageType>());
    }

    [Test]
    [TestCase("log", ConsoleMessageType.Log)]
    [TestCase("warning", ConsoleMessageType.Warning)]
    [TestCase("error", ConsoleMessageType.Error)]
    [TestCase("unity", ConsoleMessageType.UnityLog)]
    [TestCase("success", ConsoleMessageType.Success)]
    [TestCase("all", ConsoleMessageType.All)]
    public void Execute_ValidFilterName_SetsCorrectFilter(string arg, ConsoleMessageType expected)
    {
      _command.Execute(new[] { arg });
      _console.Received(1).SetLogFilter(expected);
    }

    [Test]
    public void Execute_UnknownFilter_AddsErrorMessage()
    {
      _command.Execute(new[] { "banana" });
      _console.Received().AddMessage(Arg.Is<string>(s => s.Contains("Unknown")), ConsoleMessageType.Error);
    }

    [Test]
    public void Execute_FilterNameCaseInsensitive_Works()
    {
      _command.Execute(new[] { "LOG" });
      _console.Received(1).SetLogFilter(ConsoleMessageType.Log);
    }
  }
}
