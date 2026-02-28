// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes
{
  [TestFixture]
  public class Vector3DataTests
  {
    [Test]
    public void DefaultStruct_IsZero()
    {
      var v = new Vector3Data();
      Assert.That(v.X, Is.EqualTo(0f));
      Assert.That(v.Y, Is.EqualTo(0f));
      Assert.That(v.Z, Is.EqualTo(0f));
    }

    [Test]
    public void Constructor_SetsComponents()
    {
      var v = new Vector3Data(1f, 2f, 3f);
      Assert.That(v.X, Is.EqualTo(1f));
      Assert.That(v.Y, Is.EqualTo(2f));
      Assert.That(v.Z, Is.EqualTo(3f));
    }

    [Test]
    public void Zero_IsAllZero()
    {
      Assert.That(Vector3Data.Zero.X, Is.EqualTo(0f));
      Assert.That(Vector3Data.Zero.Y, Is.EqualTo(0f));
      Assert.That(Vector3Data.Zero.Z, Is.EqualTo(0f));
    }

    [Test]
    public void One_IsAllOne()
    {
      Assert.That(Vector3Data.One.X, Is.EqualTo(1f));
      Assert.That(Vector3Data.One.Y, Is.EqualTo(1f));
      Assert.That(Vector3Data.One.Z, Is.EqualTo(1f));
    }

    [Test]
    public void Add_ReturnsSumComponents()
    {
      var a = new Vector3Data(1f, 2f, 3f);
      var b = new Vector3Data(4f, 5f, 6f);
      var result = a + b;
      Assert.That(result.X, Is.EqualTo(5f));
      Assert.That(result.Y, Is.EqualTo(7f));
      Assert.That(result.Z, Is.EqualTo(9f));
    }

    [Test]
    public void Subtract_ReturnsDiffComponents()
    {
      var a = new Vector3Data(5f, 5f, 5f);
      var b = new Vector3Data(1f, 2f, 3f);
      var result = a - b;
      Assert.That(result.X, Is.EqualTo(4f));
      Assert.That(result.Y, Is.EqualTo(3f));
      Assert.That(result.Z, Is.EqualTo(2f));
    }

    [Test]
    public void MultiplyByScalar_ScalesComponents()
    {
      var v = new Vector3Data(1f, 2f, 3f);
      var result = v * 2f;
      Assert.That(result.X, Is.EqualTo(2f));
      Assert.That(result.Y, Is.EqualTo(4f));
      Assert.That(result.Z, Is.EqualTo(6f));
    }

    [Test]
    public void ScalarMultiplyLeft_ScalesComponents()
    {
      var v = new Vector3Data(1f, 2f, 3f);
      var result = 3f * v;
      Assert.That(result.X, Is.EqualTo(3f));
      Assert.That(result.Y, Is.EqualTo(6f));
      Assert.That(result.Z, Is.EqualTo(9f));
    }

    [Test]
    public void DivideByScalar_DividesComponents()
    {
      var v = new Vector3Data(2f, 4f, 6f);
      var result = v / 2f;
      Assert.That(result.X, Is.EqualTo(1f));
      Assert.That(result.Y, Is.EqualTo(2f));
      Assert.That(result.Z, Is.EqualTo(3f));
    }

    [Test]
    public void Negate_NegatesAllComponents()
    {
      var v = new Vector3Data(1f, -2f, 3f);
      var result = -v;
      Assert.That(result.X, Is.EqualTo(-1f));
      Assert.That(result.Y, Is.EqualTo(2f));
      Assert.That(result.Z, Is.EqualTo(-3f));
    }

    [Test]
    public void Equals_SameValues_ReturnsTrue()
    {
      var a = new Vector3Data(1f, 2f, 3f);
      var b = new Vector3Data(1f, 2f, 3f);
      Assert.That(a.Equals(b), Is.True);
    }

    [Test]
    public void Equals_DifferentValues_ReturnsFalse()
    {
      var a = new Vector3Data(1f, 2f, 3f);
      var b = new Vector3Data(4f, 5f, 6f);
      Assert.That(a.Equals(b), Is.False);
    }

    [Test]
    public void Equals_NonVector3Data_ReturnsFalse()
    {
      var v = new Vector3Data(1f, 2f, 3f);
      Assert.That(v.Equals("string"), Is.False);
    }

    [Test]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
      var a = new Vector3Data(1f, 2f, 3f);
      var b = new Vector3Data(1f, 2f, 3f);
      Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }

    [Test]
    public void ToString_ReturnsFormattedString()
    {
      var v = new Vector3Data(1f, 2f, 3f);
      string result = v.ToString();
      Assert.That(result, Does.Contain("1"));
      Assert.That(result, Does.Contain("2"));
      Assert.That(result, Does.Contain("3"));
    }

    [Test]
    public void StaticConstants_Up_IsCorrect()
    {
      Assert.That(Vector3Data.Up.Y, Is.EqualTo(1f));
      Assert.That(Vector3Data.Up.X, Is.EqualTo(0f));
      Assert.That(Vector3Data.Up.Z, Is.EqualTo(0f));
    }

    [Test]
    public void StaticConstants_Forward_IsCorrect()
    {
      Assert.That(Vector3Data.Forward.Z, Is.EqualTo(1f));
    }

    [Test]
    public void StaticConstants_Right_IsCorrect()
    {
      Assert.That(Vector3Data.Right.X, Is.EqualTo(1f));
    }
  }
}
