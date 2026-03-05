// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.DevConsole.Model;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole
{
  [TestFixture]
  public class CommandHistoryTests
  {
    private CommandHistory _history;

    [SetUp]
    public void SetUp() => _history = new CommandHistory(maxCapacity: 5);

    #region Add

    [Test]
    public void Add_SingleCommand_CommandsContainsIt()
    {
      _history.Add("help");
      Assert.That(_history.Commands, Has.Count.EqualTo(1));
      Assert.That(_history.Commands[0], Is.EqualTo("help"));
    }

    [Test]
    public void Add_ExceedsCapacity_OldestIsRemoved()
    {
      for (int i = 0; i < 6; i++)
        _history.Add($"cmd{i}");

      Assert.That(_history.Commands, Has.Count.EqualTo(5));
      Assert.That(_history.Commands[0], Is.EqualTo("cmd1")); // cmd0 removed
    }

    [Test]
    public void Add_ExactlyAtCapacity_AllCommandsPresent()
    {
      for (int i = 0; i < 5; i++)
        _history.Add($"cmd{i}");

      Assert.That(_history.Commands, Has.Count.EqualTo(5));
    }

    [Test]
    public void Add_ResetsNavigationIndex()
    {
      _history.Add("first");
      _history.NavigateUp("current");   // navigate upwards
      _history.Add("second");           // addition resets the index

      // After adding navigation should restart - NavigateUp
      // will return last command, not where we stopped
      string result = _history.NavigateUp("input");
      Assert.That(result, Is.EqualTo("second"));
    }

    #endregion

    #region NavigateUp

    [Test]
    public void NavigateUp_EmptyHistory_ReturnsCurrentInput()
    {
      string result = _history.NavigateUp("typed");
      Assert.That(result, Is.EqualTo("typed"));
    }

    [Test]
    public void NavigateUp_OnFirstCall_ReturnsLastCommand()
    {
      _history.Add("first");
      _history.Add("second");

      string result = _history.NavigateUp("current");
      Assert.That(result, Is.EqualTo("second"));
    }

    [Test]
    public void NavigateUp_TwiceCalled_ReturnsSecondToLast()
    {
      _history.Add("first");
      _history.Add("second");

      _history.NavigateUp("current");
      string result = _history.NavigateUp("current");
      Assert.That(result, Is.EqualTo("first"));
    }

    [Test]
    public void NavigateUp_AtOldest_StaysAtOldest()
    {
      _history.Add("only");

      _history.NavigateUp("x");
      string result = _history.NavigateUp("x"); // one more step up - nowhere to go
      Assert.That(result, Is.EqualTo("only"));
    }

    #endregion

    #region NavigateDown

    [Test]
    public void NavigateDown_WithoutNavigatingUp_ReturnsCachedInputEmpty()
    {
      string result = _history.NavigateDown();
      // No navigation → returns empty cache
      Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void NavigateDown_AfterNavigatingUp_ReturnsToCurrentInput()
    {
      _history.Add("first");
      _history.Add("second");

      _history.NavigateUp("my input"); // stores "my input" as cache
      string result = _history.NavigateDown();
      // Down after going up first → beyond the end → cache returned
      Assert.That(result, Is.EqualTo("my input"));
    }

    [Test]
    public void NavigateDown_FromMiddle_MovesForward()
    {
      _history.Add("first");
      _history.Add("second");
      _history.Add("third");

      _history.NavigateUp("x"); // third
      _history.NavigateUp("x"); // second
      _history.NavigateUp("x"); // first

      string result = _history.NavigateDown();
      Assert.That(result, Is.EqualTo("second"));
    }

    [Test]
    public void NavigateDown_PastEnd_ReturnsOriginalInput()
    {
      _history.Add("cmd");

      _history.NavigateUp("original");
      _history.NavigateDown(); // went past the end

      // down again → still cached
      string result = _history.NavigateDown();
      Assert.That(result, Is.EqualTo("original"));
    }

    #endregion

    #region Commands (read-only)

    [Test]
    public void Commands_IsReadOnly_CannotBeModifiedDirectly()
    {
      _history.Add("test");
      var commands = _history.Commands;
      Assert.That(commands, Is.InstanceOf<System.Collections.Generic.IReadOnlyList<string>>());
    }

    [Test]
    public void Commands_EmptyHistory_ReturnsEmptyList()
    {
      Assert.That(_history.Commands, Is.Empty);
    }

    #endregion

    #region Round-trip navigation

    [Test]
    public void FullRoundTrip_NavigateUpAndDown_RestoresInput()
    {
      _history.Add("alpha");
      _history.Add("beta");
      _history.Add("gamma");

      _history.NavigateUp("current"); // gamma
      _history.NavigateUp("current"); // beta
      _history.NavigateUp("current"); // alpha

      _history.NavigateDown(); // beta
      _history.NavigateDown(); // gamma
      string result = _history.NavigateDown(); // end reached → cached

      Assert.That(result, Is.EqualTo("current"));
    }

    #endregion

  }
}
