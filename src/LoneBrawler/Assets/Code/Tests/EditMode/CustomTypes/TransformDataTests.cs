// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes
{
  [TestFixture]
  public class TransformDataTests
  {
    [Test]
    public void DefaultConstructor_SetsDefaults()
    {
      var t = new TransformData();
      Assert.That(t.Position.Equals(Vector3Data.Zero), Is.True);
      Assert.That(t.Rotation.Equals(QuatData.Identity), Is.True);
      Assert.That(t.Scale.Equals(Vector3Data.One), Is.True);
    }

    [Test]
    public void ParameterizedConstructor_SetsValues()
    {
      var pos = new Vector3Data(1f, 2f, 3f);
      var rot = QuatData.Identity;
      var scale = Vector3Data.One;
      var t = new TransformData(pos, rot, scale);
      Assert.That(t.Position.Equals(pos), Is.True);
    }

    [Test]
    public void Identity_ReturnsZeroPosIdentityRotOneScale()
    {
      var t = TransformData.Identity();
      Assert.That(t.Position.Equals(Vector3Data.Zero), Is.True);
      Assert.That(t.Rotation.Equals(QuatData.Identity), Is.True);
      Assert.That(t.Scale.Equals(Vector3Data.One), Is.True);
    }

    [Test]
    public void ToString_ContainsPositionInfo()
    {
      var t = TransformData.Identity();
      Assert.That(t.ToString(), Does.Contain("Pos"));
    }
  }
}
