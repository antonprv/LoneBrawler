// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Commands;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole.Commands
{
  [TestFixture]
  public class ClearCommandTests
  {
    private IDevConsole _console;
    private ClearCommand _command;

    [SetUp]
    public void SetUp()
    {
      _console = Substitute.For<IDevConsole>();
      _command = new ClearCommand(_console);
    }

    [Test]
    public void CommandName_IsClear()
    {
      Assert.That(_command.CommandName, Is.EqualTo("clear"));
    }

    [Test]
    public void Execute_CallsClearMessages()
    {
      _command.Execute(new string[0]);
      _console.Received(1).ClearMessages();
    }
  }
}
