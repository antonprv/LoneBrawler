// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Buffs;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData.EdgeCase
{
  [TestFixture]
  public class BuffSaveEntryEdgeCasesTests
  {
    [Test]
    public void RemainingDuration_CanBeZero()
    {
      var entry = new BuffSaveEntry
      {
        RemainingDuration = 0f
      };
      Assert.That(entry.RemainingDuration, Is.EqualTo(0f));
    }

    [Test]
    public void RemainingDuration_CanBeNegative()
    {
      // Not validated here; higher-level code should handle this
      var entry = new BuffSaveEntry
      {
        RemainingDuration = -5f
      };
      Assert.That(entry.RemainingDuration, Is.EqualTo(-5f));
    }
  }
}
