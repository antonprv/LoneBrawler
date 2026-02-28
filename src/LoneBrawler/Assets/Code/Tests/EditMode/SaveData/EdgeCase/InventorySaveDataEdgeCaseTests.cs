// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using NUnit.Framework;

namespace Code.Tests.EditMode.SaveData.EdgeCase
{
  [TestFixture]
  public class InventorySaveDataEdgeCaseTests
  {
    [Test]
    public void InitializeSlots_LargeSize_Works()
    {
      var data = new Code.Data.SaveData.Inventory.InventorySaveData();
      data.InitializeSlots(100, 10);
      Assert.That(data.InventorySlots, Has.Count.EqualTo(100));
      Assert.That(data.HotbarSlots, Has.Count.EqualTo(10));
    }

    [Test]
    public void DefaultSelectedHotbarIndex_IsZero()
    {
      var data = new Code.Data.SaveData.Inventory.InventorySaveData();
      Assert.That(data.SelectedHotbarIndex, Is.EqualTo(0));
    }
  }
}
