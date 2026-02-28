// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.DevConsole.Model;

using NUnit.Framework;

namespace Code.Tests.EditMode.DevConsole
{
  [TestFixture]
  public class ConsoleStateTests
  {
    private ConsoleState _state;

    [SetUp]
    public void SetUp() => _state = new ConsoleState();

    [Test]
    public void Default_IsNotVisible()
    {
      Assert.That(_state.IsVisible, Is.False);
    }

    [Test]
    public void Default_InputIsEmpty()
    {
      Assert.That(_state.InputText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Show_MakesVisible_ClearsInput()
    {
      _state.InputText = "some text";
      _state.Show();
      Assert.That(_state.IsVisible, Is.True);
      Assert.That(_state.InputText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Hide_MakesInvisible()
    {
      _state.Show();
      _state.Hide();
      Assert.That(_state.IsVisible, Is.False);
    }

    [Test]
    public void Toggle_WhenHidden_Shows()
    {
      _state.Toggle();
      Assert.That(_state.IsVisible, Is.True);
    }

    [Test]
    public void Toggle_WhenVisible_Hides()
    {
      _state.Show();
      _state.Toggle();
      Assert.That(_state.IsVisible, Is.False);
    }

    [Test]
    public void Toggle_TwiceCalled_ReturnsToPreviousState()
    {
      bool initialState = _state.IsVisible;
      _state.Toggle();
      _state.Toggle();
      Assert.That(_state.IsVisible, Is.EqualTo(initialState));
    }

    [Test]
    public void ClearInput_EmptiesInputText()
    {
      _state.InputText = "hello";
      _state.ClearInput();
      Assert.That(_state.InputText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void InputText_CanBeSetDirectly()
    {
      _state.InputText = "test command";
      Assert.That(_state.InputText, Is.EqualTo("test command"));
    }
  }
}
