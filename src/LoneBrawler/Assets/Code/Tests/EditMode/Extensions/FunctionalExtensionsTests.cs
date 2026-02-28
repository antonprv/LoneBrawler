// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions;

using NUnit.Framework;

namespace Code.Tests.EditMode.Extensions
{
  [TestFixture]
  public class FunctionalExtensionsTests
  {
    private class TestObject
    {
      public int Value { get; set; }
    }

    [Test]
    public void With_Action_AppliesActionAndReturnsSelf()
    {
      var obj = new TestObject { Value = 0 };
      var result = obj.With(o => o.Value = 42);

      Assert.That(result, Is.SameAs(obj));
      Assert.That(obj.Value, Is.EqualTo(42));
    }

    [Test]
    public void With_ConditionalTrue_AppliesAction()
    {
      var obj = new TestObject { Value = 0 };
      obj.With(o => o.Value = 10, when: true);
      Assert.That(obj.Value, Is.EqualTo(10));
    }

    [Test]
    public void With_ConditionalFalse_DoesNotApplyAction()
    {
      var obj = new TestObject { Value = 0 };
      obj.With(o => o.Value = 10, when: false);
      Assert.That(obj.Value, Is.EqualTo(0));
    }

    [Test]
    public void With_ConditionalFalse_ReturnsSelf()
    {
      var obj = new TestObject();
      var result = obj.With(o => o.Value = 99, when: false);
      Assert.That(result, Is.SameAs(obj));
    }

    [Test]
    public void With_Chaining_WorksCorrectly()
    {
      var obj = new TestObject { Value = 0 };
      obj.With(o => o.Value += 5).With(o => o.Value += 3);
      Assert.That(obj.Value, Is.EqualTo(8));
    }
  }
}
