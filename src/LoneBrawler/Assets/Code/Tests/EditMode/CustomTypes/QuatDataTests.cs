// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes
{
  [TestFixture]
  public class QuatDataTests
  {
    [Test]
    public void Identity_HasCorrectComponents()
    {
      var q = QuatData.Identity;
      Assert.That(q.X, Is.EqualTo(0f));
      Assert.That(q.Y, Is.EqualTo(0f));
      Assert.That(q.Z, Is.EqualTo(0f));
      Assert.That(q.W, Is.EqualTo(1f));
    }

    [Test]
    public void Constructor_SetsAllComponents()
    {
      var q = new QuatData(0.1f, 0.2f, 0.3f, 0.9f);
      Assert.That(q.X, Is.EqualTo(0.1f));
      Assert.That(q.Y, Is.EqualTo(0.2f));
      Assert.That(q.Z, Is.EqualTo(0.3f));
      Assert.That(q.W, Is.EqualTo(0.9f));
    }

    [Test]
    public void Equals_SameValues_ReturnsTrue()
    {
      var a = new QuatData(0f, 0f, 0f, 1f);
      var b = new QuatData(0f, 0f, 0f, 1f);
      Assert.That(a.Equals(b), Is.True);
    }

    [Test]
    public void Equals_DifferentValues_ReturnsFalse()
    {
      var a = new QuatData(1f, 0f, 0f, 0f);
      var b = new QuatData(0f, 0f, 0f, 1f);
      Assert.That(a.Equals(b), Is.False);
    }

    [Test]
    public void Equals_NonQuatData_ReturnsFalse()
    {
      var q = QuatData.Identity;
      Assert.That(q.Equals(42), Is.False);
    }

    [Test]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
      var a = new QuatData(0f, 0f, 0f, 1f);
      var b = new QuatData(0f, 0f, 0f, 1f);
      Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }

    [Test]
    public void ToString_ContainsComponents()
    {
      var q = new QuatData(1f, 0f, 0f, 0f);
      Assert.That(q.ToString(), Does.Contain("1"));
    }

    [Test]
    public void DefaultStruct_IsZeroNotIdentity()
    {
      var q = new QuatData();
      Assert.That(q.W, Is.EqualTo(0f));
    }
  }
}
