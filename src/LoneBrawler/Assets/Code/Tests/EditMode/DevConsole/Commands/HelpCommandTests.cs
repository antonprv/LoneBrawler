// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Infrastructure.DevConsole.Commands;
using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.DevConsole.Types;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole.Commands
{
  [TestFixture]
  public class HelpCommandTests
  {
    private IDevConsole _console;
    private Dictionary<string, IConsoleCommand> _commands;
    private HelpCommand _help;

    [SetUp]
    public void SetUp()
    {
      _console = Substitute.For<IDevConsole>();
      _commands = new Dictionary<string, IConsoleCommand>();
      _help = new HelpCommand(_console, _commands);
    }

    [Test]
    public void CommandName_IsHelp()
    {
      Assert.That(_help.CommandName, Is.EqualTo("help"));
    }

    [Test]
    public void Description_IsNotEmpty()
    {
      Assert.That(_help.Description, Is.Not.Empty);
    }

    [Test]
    public void Execute_EmptyCommands_AddsHeaderMessage()
    {
      _help.Execute(new string[0]);
      _console.Received().AddMessage(Arg.Is<string>(s => s.Contains("Available")), Arg.Any<ConsoleMessageType>());
    }

    [Test]
    public void Execute_WithRegisteredCommand_PrintsCommandInfo()
    {
      var cmd = Substitute.For<IConsoleCommand>();
      cmd.CommandName.Returns("clear");
      cmd.Description.Returns("Clears the console");
      _commands["clear"] = cmd;

      _help.Execute(new string[0]);

      _console.Received().AddMessage(
          Arg.Is<string>(s => s.Contains("clear") && s.Contains("Clears")),
          Arg.Any<ConsoleMessageType>()
      );
    }
  }
}
