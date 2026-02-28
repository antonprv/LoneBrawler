// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Buffs;
using Code.Data.StaticData.Types.Buff;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class BuffsRegistryTests
  {
    [Test]
    public void Default_PlayerBuffsIsEmpty()
    {
      var registry = new BuffsRegistry();
      Assert.That(registry.PlayerBuffs, Is.Empty);
    }

    [Test]
    public void PlayerBuffs_CanAddEntries()
    {
      var registry = new BuffsRegistry();
      registry.PlayerBuffs.Add(new BuffSaveEntry { ClassName = BuffClassName.SpeedBuff });
      Assert.That(registry.PlayerBuffs, Has.Count.EqualTo(1));
    }
  }
}
