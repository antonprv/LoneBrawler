// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.DragDropService.Types;

using NUnit.Framework;

namespace Code.Tests.EditMode.Misc
{
  [TestFixture]
  public class DragSourceTests
  {
    [Test]
    public void DragSource_HasExpectedValues()
    {
      var values = System.Enum.GetValues(typeof(DragSource));
      Assert.That(values.Length, Is.GreaterThan(0));
    }

    [Test]
    public void DragSource_None_HasDefaultValue()
    {
      Assert.That((int)DragSource.None, Is.EqualTo(0));
    }
  }
}
