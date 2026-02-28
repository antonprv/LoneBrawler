// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Enemies;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class EnemiesKilledTests
  {
    [Test]
    public void Default_ClearedSpawnersIsEmpty()
    {
      var ek = new EnemiesKilled();
      Assert.That(ek.ClearedSpawners, Is.Not.Null);
      Assert.That(ek.ClearedSpawners, Is.Empty);
    }

    [Test]
    public void CanAddSpawner()
    {
      var ek = new EnemiesKilled();
      ek.ClearedSpawners.Add("spawner_001");
      Assert.That(ek.ClearedSpawners, Has.Count.EqualTo(1));
    }

    [Test]
    public void DuplicateSpawner_NotAdded_IsHashSet()
    {
      var ek = new EnemiesKilled();
      ek.ClearedSpawners.Add("spawner_001");
      ek.ClearedSpawners.Add("spawner_001");
      Assert.That(ek.ClearedSpawners, Has.Count.EqualTo(1));
    }
  }
}
