// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes.Collections
{
  [TestFixture]
  public class PairDataTests
  {
    [Test]
    public void DefaultConstructor_DefaultValues()
    {
      var pair = new PairData<string, int>();
      Assert.That(pair.Key, Is.Null);
      Assert.That(pair.Value, Is.EqualTo(0));
    }

    [Test]
    public void ParameterizedConstructor_SetsKeyAndValue()
    {
      var pair = new PairData<string, int>("myKey", 42);
      Assert.That(pair.Key, Is.EqualTo("myKey"));
      Assert.That(pair.Value, Is.EqualTo(42));
    }

    [Test]
    public void Key_CanBeSetAfterConstruction()
    {
      var pair = new PairData<string, int>();
      pair.Key = "updated";
      Assert.That(pair.Key, Is.EqualTo("updated"));
    }

    [Test]
    public void Value_CanBeSetAfterConstruction()
    {
      var pair = new PairData<string, int>();
      pair.Value = 99;
      Assert.That(pair.Value, Is.EqualTo(99));
    }

    [Test]
    public void ParameterizedConstructor_WithIntKey()
    {
      var pair = new PairData<int, float>(10, 3.14f);
      Assert.That(pair.Key, Is.EqualTo(10));
      Assert.That(pair.Value, Is.EqualTo(3.14f).Within(0.001f));
    }
  }
}
