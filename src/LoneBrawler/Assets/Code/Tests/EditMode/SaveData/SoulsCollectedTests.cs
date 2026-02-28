// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class SoulsCollectedTests
  {
    [Test]
    public void Default_AmountIsZero()
    {
      var sc = new SoulsCollected();
      Assert.That(sc.Amount, Is.EqualTo(0));
    }

    [Test]
    public void Default_LeftSpawnersIsNotNull()
    {
      var sc = new SoulsCollected();
      Assert.That(sc.LeftSpawners, Is.Not.Null);
    }

    [Test]
    public void Amount_CanBeModified()
    {
      var sc = new SoulsCollected();
      sc.Amount += 100;
      Assert.That(sc.Amount, Is.EqualTo(100));
    }
  }
}
