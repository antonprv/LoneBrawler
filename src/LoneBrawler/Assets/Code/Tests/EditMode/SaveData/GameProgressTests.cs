// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class GameProgressTests
  {
    private IPlayerDataSubervice _playerData;

    [SetUp]
    public void SetUp()
    {
      _playerData = SaveDataTestHelpers.MakePlayerData();
    }

    [Test]
    public void Constructor_InitializesAllSections()
    {
      var progress = new GameProgress(_playerData, "Level_01");

      Assert.That(progress.PlayerWorldData, Is.Not.Null);
      Assert.That(progress.PLayerState, Is.Not.Null);
      Assert.That(progress.PlayerStats, Is.Not.Null);
      Assert.That(progress.EnemiesKilled, Is.Not.Null);
      Assert.That(progress.SoulsCollected, Is.Not.Null);
      Assert.That(progress.BuffsRegistry, Is.Not.Null);
    }

    [Test]
    public void Constructor_SetsSaveTimeToZero()
    {
      var progress = new GameProgress(_playerData, "Level_01");
      Assert.That(progress.SaveTimeUTC, Is.EqualTo(0L));
    }

    [Test]
    public void CurrentScene_ReturnsInitialLevelName()
    {
      var progress = new GameProgress(_playerData, "Level_Forest");
      Assert.That(progress.CurrentScene, Is.EqualTo("Level_Forest"));
    }

    [Test]
    public void IsWorldDataValid_WithFreshProgress_ReturnsFalse()
    {
      // TransformOnLevel.Transform = null by default → IsValid() = false
      var progress = new GameProgress(_playerData, "Level_01");
      Assert.That(progress.IsWorldDataValid(), Is.False);
    }

    [Test]
    public void IsPlayerStatsValid_WithValidStats_ReturnsTrue()
    {
      var progress = new GameProgress(_playerData, "Level_01");
      Assert.That(progress.IsPlayerStatsValid(), Is.True);
    }

    [Test]
    public void IsPlayerDataValid_WithValidState_ReturnsTrue()
    {
      var progress = new GameProgress(_playerData, "Level_01");
      Assert.That(progress.IsPlayerDataValid(), Is.True);
    }

    [Test]
    public void IsPlayerStatsValid_NullStats_ReturnsFalse()
    {
      var progress = new GameProgress(_playerData, "Level_01");
      progress.PlayerStats = null;
      Assert.That(progress.IsPlayerStatsValid(), Is.False);
    }

    [Test]
    public void IsPlayerDataValid_NullState_ReturnsFalse()
    {
      var progress = new GameProgress(_playerData, "Level_01");
      progress.PLayerState = null;
      Assert.That(progress.IsPlayerDataValid(), Is.False);
    }
  }
}
