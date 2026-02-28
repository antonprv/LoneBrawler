// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Buffs;
using Code.Data.StaticData.Types.Buff;

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData
{
  [TestFixture]
  public class BuffSaveEntryTests
  {
    [Test]
    public void CanSetAllFields()
    {
      var entry = new BuffSaveEntry
      {
        ClassName = BuffClassName.RageBuff,
        ActivationType = BuffActivationType.Duration,
        State = BuffState.Active,
        RemainingDuration = 7.5f
      };

      Assert.That(entry.ClassName, Is.EqualTo(BuffClassName.RageBuff));
      Assert.That(entry.ActivationType, Is.EqualTo(BuffActivationType.Duration));
      Assert.That(entry.State, Is.EqualTo(BuffState.Active));
      Assert.That(entry.RemainingDuration, Is.EqualTo(7.5f));
    }
  }
}
