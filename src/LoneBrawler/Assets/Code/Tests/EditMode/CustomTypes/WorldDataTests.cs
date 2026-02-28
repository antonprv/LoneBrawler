// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;
using Code.Data.SaveData.Types;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes
{
  [TestFixture]
  public class WorldDataTests
  {
    [Test]
    public void Constructor_SetsTransformOnLevel()
    {
      var tol = new TransformOnLevel("Level_01");
      var wd = new WorldData(tol);
      Assert.That(wd.TransformOnLevel, Is.SameAs(tol));
    }

    [Test]
    public void Constructor_SetsDefaultsForTeleport()
    {
      var tol = new TransformOnLevel("L");
      var wd = new WorldData(tol);
      Assert.That(wd.LastTeleportUniqueName, Is.Null);
      Assert.That(wd.LastTeleportTimeUTC, Is.EqualTo(0L));
    }

    [Test]
    public void IsValid_WithNullTransformOnLevel_ReturnsFalse()
    {
      var wd = new WorldData(null);
      Assert.That(wd.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_WithInvalidTransformOnLevel_ReturnsFalse()
    {
      // TransformOnLevel without transform → invalid
      var tol = new TransformOnLevel("Level_01");
      var wd = new WorldData(tol);
      Assert.That(wd.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_WithValidTransformOnLevel_ReturnsTrue()
    {
      var tol = new TransformOnLevel(TransformData.Identity(), "Level_01");
      var wd = new WorldData(tol);
      Assert.That(wd.IsValid(), Is.True);
    }
  }
}
