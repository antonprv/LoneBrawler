// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;
using Code.Data.SaveData.Types;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes
{
  [TestFixture]
  public class TransformOnLevelTests
  {
    [Test]
    public void Constructor_LevelNameOnly_SetsLevelName()
    {
      var t = new TransformOnLevel("Level_01");
      Assert.That(t.LevelName, Is.EqualTo("Level_01"));
    }

    [Test]
    public void IsValid_WithNullTransform_ReturnsFalse()
    {
      var t = new TransformOnLevel("Level_01"); // Transform = null
      Assert.That(t.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_WithTransformAndEmptyName_ReturnsFalse()
    {
      var t = new TransformOnLevel(TransformData.Identity(), "");
      Assert.That(t.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_WithTransformAndWhitespaceName_ReturnsFalse()
    {
      var t = new TransformOnLevel(TransformData.Identity(), "   ");
      Assert.That(t.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_WithTransformAndValidName_ReturnsTrue()
    {
      var t = new TransformOnLevel(TransformData.Identity(), "Level_01");
      Assert.That(t.IsValid(), Is.True);
    }

    [Test]
    public void FullConstructor_SetsTransformAndName()
    {
      var transform = TransformData.Identity();
      var t = new TransformOnLevel(transform, "Level_02");
      Assert.That(t.LevelName, Is.EqualTo("Level_02"));
      Assert.That(t.Transform, Is.SameAs(transform));
    }
  }
}
