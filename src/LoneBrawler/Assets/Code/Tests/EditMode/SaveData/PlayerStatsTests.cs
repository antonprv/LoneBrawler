// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Player;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class PlayerStatsTests
  {
    [Test]
    public void Constructor_SetsAllValuesFromPlayerData()
    {
      var data = SaveDataTestHelpers.MakePlayerData(
          speed: 7f, rotSpeed: 4f, damage: 15f, range: 8f, radius: 3f, maxEnemies: 5);

      var stats = new PlayerStats(data);

      Assert.That(stats.MovementSpeed, Is.EqualTo(7f));
      Assert.That(stats.RotationSpeed, Is.EqualTo(4f));
      Assert.That(stats.Damage, Is.EqualTo(15f));
      Assert.That(stats.Range, Is.EqualTo(8f));
      Assert.That(stats.Radius, Is.EqualTo(3f));
      Assert.That(stats.MaxEnemiesHit, Is.EqualTo(5));
    }

    [Test]
    public void IsValid_AllNonZero_ReturnsTrue()
    {
      var stats = new PlayerStats(SaveDataTestHelpers.MakePlayerData());
      Assert.That(stats.IsValid(), Is.True);
    }

    [Test]
    public void IsValid_ZeroDamage_ReturnsFalse()
    {
      var stats = new PlayerStats(SaveDataTestHelpers.MakePlayerData(damage: 0f));
      Assert.That(stats.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_ZeroRange_ReturnsFalse()
    {
      var stats = new PlayerStats(SaveDataTestHelpers.MakePlayerData(range: 0f));
      Assert.That(stats.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_ZeroRadius_ReturnsFalse()
    {
      var stats = new PlayerStats(SaveDataTestHelpers.MakePlayerData(radius: 0f));
      Assert.That(stats.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_ZeroMaxEnemiesHit_ReturnsFalse()
    {
      var stats = new PlayerStats(SaveDataTestHelpers.MakePlayerData(maxEnemies: 0));
      Assert.That(stats.IsValid(), Is.False);
    }
  }
}
