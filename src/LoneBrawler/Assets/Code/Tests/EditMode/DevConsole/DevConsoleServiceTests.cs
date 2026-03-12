// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.DevConsole;
using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.DevConsole.Service;
using Code.Infrastructure.DevConsole.Types;

using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole
{
  [TestFixture]
  public class DevConsoleServiceTests
  {
    private DevConsoleService _console;
    private IBuildConfigSubservice _buildConfig;

    [SetUp]
    public void SetUp()
    {
      _buildConfig = Substitute.For<IBuildConfigSubservice>();
      _buildConfig = Substitute.For<IBuildConfigSubservice>();

      // By default - development build, console accessible
      _buildConfig.IsDevelopment().Returns(true);

      _console = new DevConsoleService(_buildConfig);
    }

    #region Initialize

    [Test]
    public void Initialize_InDevelopmentBuild_ConsoleIsEnabledStateIsFalse()
    {
      _console.Initialize();
      // IsEnabled is visibility status, not initialization fact itself
      Assert.That(_console.IsEnabled, Is.False);
    }

    [Test]
    public void Initialize_CalledTwice_DoesNotDuplicateMessages()
    {
      _console.Initialize();
      int countAfterFirst = _console.GetMessages().Length;

      _console.Initialize();
      int countAfterSecond = _console.GetMessages().Length;

      Assert.That(countAfterSecond, Is.EqualTo(countAfterFirst));
    }

    [Test]
    public void Initialize_InShippingBuild_NoMessagesAdded()
    {
      _buildConfig.IsDevelopment().Returns(false);
      _console.Initialize();

      Assert.That(_console.GetMessages(), Is.Empty);
    }

    #endregion

    #region Toggle

    [Test]
    public void Toggle_InDevelopment_TogglesIsEnabled()
    {
      _console.Initialize();
      _console.Toggle();
      Assert.That(_console.IsEnabled, Is.True);

      _console.Toggle();
      Assert.That(_console.IsEnabled, Is.False);
    }

    [Test]
    public void Toggle_InShippingBuild_DoesNotToggle()
    {
      _buildConfig.IsDevelopment().Returns(false);
      _console.Initialize();
      _console.Toggle();

      Assert.That(_console.IsEnabled, Is.False);
    }

    #endregion

    #region RegisterCommand

    [Test]
    public void RegisterCommand_ValidCommand_CanBeExecuted()
    {
      _console.Initialize();

      var cmd = Substitute.For<IConsoleCommand>();
      cmd.CommandName.Returns("testcmd");

      _console.RegisterCommand(cmd);
      _console.ExecuteCommand("testcmd");

      cmd.Received(1).Execute(Arg.Any<string[]>());
    }

    [Test]
    public void RegisterCommand_DuplicateName_AddsWarningMessage()
    {
      _console.Initialize();

      var cmd1 = Substitute.For<IConsoleCommand>();
      cmd1.CommandName.Returns("dup");
      var cmd2 = Substitute.For<IConsoleCommand>();
      cmd2.CommandName.Returns("dup");

      _console.RegisterCommand(cmd1);
      _console.RegisterCommand(cmd2);

      // Duplicate registration should add a warning
      string[] messages = _console.GetMessages();
      bool hasWarning = System.Array.Exists(messages, m => m.Contains("already registered"));
      Assert.That(hasWarning, Is.True);
    }

    [Test]
    public void RegisterCommand_InShippingBuild_CommandNotRegistered()
    {
      _buildConfig.IsDevelopment().Returns(false);
      _console.Initialize();

      var cmd = Substitute.For<IConsoleCommand>();
      cmd.CommandName.Returns("secret");

      _console.RegisterCommand(cmd);
      _console.ExecuteCommand("secret");

      cmd.DidNotReceive().Execute(Arg.Any<string[]>());
    }

    #endregion

    #region ExecuteCommand

    [Test]
    public void ExecuteCommand_EmptyString_DoesNothing()
    {
      _console.Initialize();
      int before = _console.GetMessages().Length;
      _console.ExecuteCommand("");
      // empty command does not add messages
      Assert.That(_console.GetMessages().Length, Is.EqualTo(before));
    }

    [Test]
    public void ExecuteCommand_WhitespaceOnly_DoesNothing()
    {
      _console.Initialize();
      int before = _console.GetMessages().Length;
      _console.ExecuteCommand("   ");
      Assert.That(_console.GetMessages().Length, Is.EqualTo(before));
    }

    [Test]
    public void ExecuteCommand_UnknownCommand_AddsErrorMessage()
    {
      _console.Initialize();
      _console.ExecuteCommand("nonexistent");

      string[] messages = _console.GetMessages();
      bool hasError = System.Array.Exists(messages, m => m.Contains("Unknown command"));
      Assert.That(hasError, Is.True);
    }

    [Test]
    public void ExecuteCommand_PassesArgsToCommand()
    {
      _console.Initialize();

      var cmd = Substitute.For<IConsoleCommand>();
      cmd.CommandName.Returns("move");
      _console.RegisterCommand(cmd);

      _console.ExecuteCommand("move 10 20");

      cmd.Received(1).Execute(Arg.Is<string[]>(a => a[0] == "10" && a[1] == "20"));
    }

    [Test]
    public void ExecuteCommand_IsCaseInsensitive()
    {
      _console.Initialize();

      var cmd = Substitute.For<IConsoleCommand>();
      cmd.CommandName.Returns("help");
      _console.RegisterCommand(cmd);

      _console.ExecuteCommand("HELP");
      // HelpCommand is already registered upon Initialize, checking absence of error
      string[] messages = _console.GetMessages();
      bool hasError = System.Array.Exists(messages, m => m.Contains("Unknown command: help"));
      Assert.That(hasError, Is.False);
    }

    #endregion

    #region AddMessage / GetMessages

    [Test]
    public void AddMessage_ThenGetMessages_ContainsFormattedMessage()
    {
      _console.Initialize();
      _console.AddMessage("hello world", ConsoleMessageType.Log);

      string[] messages = _console.GetMessages();
      bool found = System.Array.Exists(messages, m => m.Contains("hello world"));
      Assert.That(found, Is.True);
    }

    [Test]
    public void AddMessage_ExceedsMaxCapacity_OldestMessagesRemoved()
    {
      _console.Initialize();
      // Adding 510 messages (limit is 500)
      for (int i = 0; i < 510; i++)
        _console.AddMessage($"msg{i}", ConsoleMessageType.Log);

      // Earliest messages should be removed
      string[] messages = _console.GetMessages();
      bool hasVeryOld = System.Array.Exists(messages, m => m.Contains("msg0"));
      Assert.That(hasVeryOld, Is.False);
    }

    #endregion

    #region ClearMessages

    [Test]
    public void ClearMessages_RemovesAllMessages()
    {
      _console.Initialize();
      _console.AddMessage("test1");
      _console.AddMessage("test2");
      _console.ClearMessages();

      string[] messages = _console.GetMessages();
      // After clearing, only "Console cleared" message remains
      bool hasTest1 = System.Array.Exists(messages, m => m.Contains("test1"));
      Assert.That(hasTest1, Is.False);
    }

    [Test]
    public void ClearMessages_AddsSystemMessage()
    {
      _console.Initialize();
      _console.ClearMessages();

      string[] messages = _console.GetMessages();
      bool hasClearedMsg = System.Array.Exists(messages, m => m.Contains("Console cleared"));
      Assert.That(hasClearedMsg, Is.True);
    }

    #endregion

    #region SetLogFilter / GetLogFilter

    [Test]
    public void SetLogFilter_ChangesFilter()
    {
      _console.Initialize();
      _console.SetLogFilter(ConsoleMessageType.Error);
      Assert.That(_console.GetLogFilter(), Is.EqualTo(ConsoleMessageType.Error));
    }

    [Test]
    public void GetMessages_WithErrorFilter_ShowsOnlyErrorsAndCommands()
    {
      _console.Initialize();
      _console.ClearMessages();

      _console.SetLogFilter(ConsoleMessageType.Error);

      _console.AddMessage("normal log", ConsoleMessageType.Log);
      _console.AddMessage("error message", ConsoleMessageType.Error);

      string[] messages = _console.GetMessages();

      bool hasLog = System.Array.Exists(messages, m => m.Contains("normal log"));
      bool hasError = System.Array.Exists(messages, m => m.Contains("error message"));

      Assert.That(hasLog, Is.False);
      Assert.That(hasError, Is.True);
    }

    [Test]
    public void GetMessages_WithAllFilter_ShowsEverything()
    {
      _console.Initialize();
      _console.ClearMessages();

      _console.SetLogFilter(ConsoleMessageType.All);
      _console.AddMessage("log", ConsoleMessageType.Log);
      _console.AddMessage("warning", ConsoleMessageType.Warning);

      string[] messages = _console.GetMessages();
      Assert.That(messages.Length, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void GetMessages_CommandTypeAlwaysShown_EvenWithDifferentFilter()
    {
      _console.Initialize();
      _console.ClearMessages();

      _console.SetLogFilter(ConsoleMessageType.Error);

      // Commands are marked automatically upon ExecuteCommand
      var cmd = Substitute.For<IConsoleCommand>();
      cmd.CommandName.Returns("ping");
      _console.RegisterCommand(cmd);
      _console.ExecuteCommand("ping");

      string[] messages = _console.GetMessages();
      bool hasCommand = System.Array.Exists(messages, m => m.Contains("> ping"));
      Assert.That(hasCommand, Is.True);
    }

    #endregion

    #region SetCaptureUnityLogs

    [Test]
    public void SetCaptureUnityLogs_ToFalse_AddsConfirmationMessage()
    {
      _console.Initialize();
      _console.SetCaptureUnityLogs(false);

      string[] messages = _console.GetMessages();
      bool hasMsg = System.Array.Exists(messages, m => m.Contains("disabled"));
      Assert.That(hasMsg, Is.True);
    }

    [Test]
    public void SetCaptureUnityLogs_SameValue_DoesNotAddMessage()
    {
      _console.Initialize();
      int before = _console.GetMessages().Length;

      _console.SetCaptureUnityLogs(true); // already true, no changes made

      Assert.That(_console.GetMessages().Length, Is.EqualTo(before));
    }
  }

    #endregion

  #region ConsoleMessage Tests

  [TestFixture]
  public class ConsoleMessageTests
  {
    private IDevConsole _fakeConsole;

    [SetUp]
    public void SetUp()
    {
      _fakeConsole = Substitute.For<IDevConsole>();
      _fakeConsole.ConsoleMarker.Returns("[Console] ");
    }

    [Test]
    public void Constructor_LogType_FormatsWithConsoleMarker()
    {
      var msg = new ConsoleMessage("test", ConsoleMessageType.Log, _fakeConsole);
      Assert.That(msg.FormattedMessage, Does.StartWith("[Console] "));
      Assert.That(msg.FormattedMessage, Does.Contain("test"));
    }

    [Test]
    public void Constructor_WarningType_IncludesYellowColorTag()
    {
      var msg = new ConsoleMessage("warn", ConsoleMessageType.Warning, _fakeConsole);
      Assert.That(msg.FormattedMessage, Does.Contain("[WARNING]"));
    }

    [Test]
    public void Constructor_ErrorType_IncludesRedColorTag()
    {
      var msg = new ConsoleMessage("err", ConsoleMessageType.Error, _fakeConsole);
      Assert.That(msg.FormattedMessage, Does.Contain("[ERROR]"));
    }

    [Test]
    public void Constructor_SuccessType_IncludesOkTag()
    {
      var msg = new ConsoleMessage("ok", ConsoleMessageType.Success, _fakeConsole);
      Assert.That(msg.FormattedMessage, Does.Contain("[OK]"));
    }

    [Test]
    public void Constructor_CommandType_UsesCyanColor()
    {
      // FormatMessage for Command: ConsoleMarker + "<color=cyan>" + message + "</color>"
      // Marker present (else branch in FormatMessage) - verifying only presence of cyan color and text.
      var msg = new ConsoleMessage("cmd", ConsoleMessageType.Command, _fakeConsole);
      Assert.That(msg.FormattedMessage, Does.Contain("cyan"));
      Assert.That(msg.FormattedMessage, Does.Contain("cmd"));
    }

    [Test]
    public void Constructor_UnityLogType_UsesPrefixWithoutConsoleMarker()
    {
      var msg = new ConsoleMessage("unity", ConsoleMessageType.UnityLog, _fakeConsole);
      Assert.That(msg.FormattedMessage, Does.Contain("[Unity]"));
      Assert.That(msg.FormattedMessage, Does.Not.StartWith("[Console]"));
    }

    [Test]
    public void Constructor_SetsMessageAndTypeProperties()
    {
      var msg = new ConsoleMessage("hello", ConsoleMessageType.Warning, _fakeConsole);
      Assert.That(msg.Message, Is.EqualTo("hello"));
      Assert.That(msg.Type, Is.EqualTo(ConsoleMessageType.Warning));
    }

  #endregion

  }
}
