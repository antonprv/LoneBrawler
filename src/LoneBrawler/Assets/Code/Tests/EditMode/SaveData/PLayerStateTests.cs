// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Player;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class PLayerStateTests
  {
    [Test]
    public void Constructor_SetsMaxHealthFromPlayerData()
    {
      var data = SaveDataTestHelpers.MakePlayerData(maxHealth: 200f);
      var state = new PLayerState(data);
      Assert.That(state.MaxHealth, Is.EqualTo(200f));
    }

    [Test]
    public void Constructor_SetsCurrentHealthToMaxHealth()
    {
      var data = SaveDataTestHelpers.MakePlayerData(maxHealth: 150f);
      var state = new PLayerState(data);
      Assert.That(state.CurrentHealth, Is.EqualTo(150f));
    }

    [Test]
    public void IsValid_BothNonZero_ReturnsTrue()
    {
      var data = SaveDataTestHelpers.MakePlayerData(maxHealth: 100f);
      var state = new PLayerState(data);
      Assert.That(state.IsValid(), Is.True);
    }

    [Test]
    public void IsValid_ZeroMaxHealth_ReturnsFalse()
    {
      var data = SaveDataTestHelpers.MakePlayerData(maxHealth: 0f);
      var state = new PLayerState(data);
      Assert.That(state.IsValid(), Is.False);
    }

    [Test]
    public void IsValid_CurrentHealthManuallySetToZero_ReturnsFalse()
    {
      var data = SaveDataTestHelpers.MakePlayerData(maxHealth: 100f);
      var state = new PLayerState(data);
      state.CurrentHealth = 0f;
      Assert.That(state.IsValid(), Is.False);
    }
  }
}
