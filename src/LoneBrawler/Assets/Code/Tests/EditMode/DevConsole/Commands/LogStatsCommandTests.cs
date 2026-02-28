// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Types;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole.Commands
{
  [TestFixture]
  public class LogStatsCommandTests
  {
    private IDevConsole _console;
    private LogStatsCommand _command;

    [SetUp]
    public void SetUp()
    {
      _console = Substitute.For<IDevConsole>();
      _command = new LogStatsCommand(_console);
    }

    [Test]
    public void CommandName_IsLogStats()
    {
      Assert.That(_command.CommandName, Is.EqualTo("log_stats"));
    }

    [Test]
    public void Description_IsNotEmpty()
    {
      Assert.That(_command.Description, Is.Not.Empty);
    }

    [Test]
    public void Execute_AddsFourMessages()
    {
      _command.Execute(new string[0]);
      _console.Received(4).AddMessage(
          Arg.Any<string>(),
          Arg.Any<ConsoleMessageType>()
      );
    }

    [Test]
    public void Execute_FirstMessageContainsStatistics()
    {
      _command.Execute(new string[0]);
      _console.Received().AddMessage(
          Arg.Is<string>(s => s.Contains("Statistics") || s.Contains("Log")),
          Arg.Any<ConsoleMessageType>()
      );
    }
  }
}
