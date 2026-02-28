// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;

using NUnit.Framework;

namespace Code.Tests.EditMode.Inventory
{
  [TestFixture]
  public class InventorySlotDataTests
  {
    #region Constructor

    [Test]
    public void DefaultConstructor_SetsNoneAndZero()
    {
      var slot = new InventorySlotData();
      Assert.That(slot.BuffClass, Is.EqualTo(BuffClassName.None));
      Assert.That(slot.Count, Is.EqualTo(0));
    }

    [Test]
    public void ParameterizedConstructor_SetsValues()
    {
      var slot = new InventorySlotData(BuffClassName.HealthBuff, 3);
      Assert.That(slot.BuffClass, Is.EqualTo(BuffClassName.HealthBuff));
      Assert.That(slot.Count, Is.EqualTo(3));
    }

    #endregion

    #region IsEmpty

    [Test]
    public void IsEmpty_DefaultSlot_ReturnsTrue()
    {
      var slot = new InventorySlotData();
      Assert.That(slot.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_NoneWithCount_ReturnsTrue()
    {
      var slot = new InventorySlotData(BuffClassName.None, 5);
      Assert.That(slot.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_ValidBuffZeroCount_ReturnsTrue()
    {
      var slot = new InventorySlotData(BuffClassName.DamageBuff, 0);
      Assert.That(slot.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_ValidBuffWithCount_ReturnsFalse()
    {
      var slot = new InventorySlotData(BuffClassName.SpeedBuff, 1);
      Assert.That(slot.IsEmpty, Is.False);
    }

    #endregion

    #region Clear

    [Test]
    public void Clear_FilledSlot_BecomesEmpty()
    {
      var slot = new InventorySlotData(BuffClassName.RageBuff, 2);
      slot.Clear();

      Assert.That(slot.IsEmpty, Is.True);
      Assert.That(slot.BuffClass, Is.EqualTo(BuffClassName.None));
      Assert.That(slot.Count, Is.EqualTo(0));
    }

    [Test]
    public void Clear_AlreadyEmpty_RemainsEmpty()
    {
      var slot = new InventorySlotData();
      slot.Clear();
      Assert.That(slot.IsEmpty, Is.True);
    }

    #endregion

    #region Set

    [Test]
    public void Set_ValidValues_UpdatesSlot()
    {
      var slot = new InventorySlotData();
      slot.Set(BuffClassName.GodBuff, 5);

      Assert.That(slot.BuffClass, Is.EqualTo(BuffClassName.GodBuff));
      Assert.That(slot.Count, Is.EqualTo(5));
    }

    [Test]
    public void Set_CalledTwice_OverwritesPreviousValues()
    {
      var slot = new InventorySlotData();
      slot.Set(BuffClassName.HealthBuff, 3);
      slot.Set(BuffClassName.RegenBuff, 7);

      Assert.That(slot.BuffClass, Is.EqualTo(BuffClassName.RegenBuff));
      Assert.That(slot.Count, Is.EqualTo(7));
    }

    [Test]
    public void Set_NoneBuffClass_SlotBecomesEmpty()
    {
      var slot = new InventorySlotData(BuffClassName.SpeedBuff, 2);
      slot.Set(BuffClassName.None, 1);

      Assert.That(slot.IsEmpty, Is.True);
    }

    #endregion

    #region All BuffClassName values

    [Test]
    [TestCase(BuffClassName.DamageBuff)]
    [TestCase(BuffClassName.RageBuff)]
    [TestCase(BuffClassName.GodBuff)]
    [TestCase(BuffClassName.SpeedBuff)]
    [TestCase(BuffClassName.RegenBuff)]
    [TestCase(BuffClassName.HealthBuff)]
    [TestCase(BuffClassName.HealthPotionBuff)]
    public void IsEmpty_WithAnyValidBuff_AndPositiveCount_ReturnsFalse(BuffClassName buffClass)
    {
      var slot = new InventorySlotData(buffClass, 1);
      Assert.That(slot.IsEmpty, Is.False);
    }

    #endregion
  }
}
