// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Inventory;

using NUnit.Framework;

namespace Code.Tests.EditMode.Inventory
{
  [TestFixture]
  public class InventorySaveDataTests
  {
    #region Default constructor

    [Test]
    public void DefaultConstructor_CreatesEmptyLists()
    {
      var data = new InventorySaveData();
      Assert.That(data.InventorySlots, Is.Not.Null);
      Assert.That(data.HotbarSlots, Is.Not.Null);
      Assert.That(data.InventorySlots, Is.Empty);
      Assert.That(data.HotbarSlots, Is.Empty);
      Assert.That(data.SelectedHotbarIndex, Is.EqualTo(0));
    }

    #endregion

    #region InitializeSlots

    [Test]
    public void InitializeSlots_CreatesCorrectNumberOfSlots()
    {
      var data = new InventorySaveData();
      data.InitializeSlots(inventorySize: 10, hotbarSize: 4);

      Assert.That(data.InventorySlots, Has.Count.EqualTo(10));
      Assert.That(data.HotbarSlots, Has.Count.EqualTo(4));
    }

    [Test]
    public void InitializeSlots_AllSlotsAreEmpty()
    {
      var data = new InventorySaveData();
      data.InitializeSlots(5, 3);

      foreach (var slot in data.InventorySlots)
        Assert.That(slot.IsEmpty, Is.True);

      foreach (var slot in data.HotbarSlots)
        Assert.That(slot.IsEmpty, Is.True);
    }

    [Test]
    public void InitializeSlots_ZeroSize_CreatesEmptyList()
    {
      var data = new InventorySaveData();
      data.InitializeSlots(0, 0);

      Assert.That(data.InventorySlots, Is.Empty);
      Assert.That(data.HotbarSlots, Is.Empty);
    }

    [Test]
    public void InitializeSlots_CalledTwice_OverwritesPrevious()
    {
      var data = new InventorySaveData();
      data.InitializeSlots(5, 3);
      data.InitializeSlots(2, 1);

      Assert.That(data.InventorySlots, Has.Count.EqualTo(2));
      Assert.That(data.HotbarSlots, Has.Count.EqualTo(1));
    }

    #endregion

  }
}
