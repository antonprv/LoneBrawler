// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.UI.Services.InventoryService;

using Code.Common.Extensions.Logging;
using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NSubstitute;

using NUnit.Framework;

using R3;

namespace Code.Tests.EditMode.Inventory
{
  [TestFixture]
  public class InventoryServiceTests
  {
    private InventoryService _service;
    private IBuffDataSubservice _buffDataSubservice;
    private IGameLog _logger;

    private const int InventorySize = 10;
    private const int HotbarSize = 4;

    [SetUp]
    public void SetUp()
    {
      _buffDataSubservice = Substitute.For<IBuffDataSubservice>();
      _logger = Substitute.For<IGameLog>();
      _service = new InventoryService(_buffDataSubservice, _logger);
      _service.Initialize(InventorySize, HotbarSize);
    }

    #region Initialization

    [Test]
    public void Initialize_SetsCorrectSizes()
    {
      Assert.That(_service.InventorySize, Is.EqualTo(InventorySize));
      Assert.That(_service.HotbarSize, Is.EqualTo(HotbarSize));
    }

    [Test]
    public void Initialize_AllSlotAreEmpty()
    {
      for (int i = 0; i < InventorySize; i++)
        Assert.That(_service.GetInventorySlot(i).IsEmpty, Is.True);
      for (int i = 0; i < HotbarSize; i++)
        Assert.That(_service.GetHotbarSlot(i).IsEmpty, Is.True);
    }

    [Test]
    public void Initialize_ByDefaultHotbarIsNotSelected()
    {
      Assert.That(_service.SelectedHotbarIndex, Is.EqualTo(-1));
    }

    #endregion

    #region GetInventorySlot / GetHotbarSlot

    [Test]
    public void GetInventorySlot_ValidIndex_ReturnsSlot()
    {
      var slot = _service.GetInventorySlot(0);
      Assert.That(slot, Is.Not.Null);
    }

    [Test]
    public void GetInventorySlot_NegativeIndex_ReturnsNull()
    {
      var slot = _service.GetInventorySlot(-1);
      Assert.That(slot, Is.Null);
    }

    [Test]
    public void GetInventorySlot_TooLargeIndex_ReturnsNull()
    {
      var slot = _service.GetInventorySlot(InventorySize);
      Assert.That(slot, Is.Null);
    }

    [Test]
    public void GetHotbarSlot_ValidIndex_ReturnsSlot()
    {
      Assert.That(_service.GetHotbarSlot(0), Is.Not.Null);
    }

    [Test]
    public void GetHotbarSlot_OutOfRange_ReturnsNull()
    {
      Assert.That(_service.GetHotbarSlot(HotbarSize), Is.Null);
    }

    #endregion

    #region HasBuff / GetBuffCount

    [Test]
    public void HasBuff_EmptyInventory_ReturnsFalse()
    {
      Assert.That(_service.HasBuff(BuffClassName.SpeedBuff), Is.False);
    }

    [Test]
    public void GetBuffCount_EmptyInventory_ReturnsZero()
    {
      Assert.That(_service.GetBuffCount(BuffClassName.SpeedBuff), Is.EqualTo(0));
    }

    [Test]
    public void HasBuff_AfterManualSlotSet_ReturnsTrue()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.SpeedBuff, 2);
      Assert.That(_service.HasBuff(BuffClassName.SpeedBuff), Is.True);
    }

    [Test]
    public void GetBuffCount_CountsAcrossAllSlots()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.HealthBuff, 3);
      _service.GetInventorySlot(2).Set(BuffClassName.HealthBuff, 2);
      _service.GetHotbarSlot(0).Set(BuffClassName.HealthBuff, 1);

      Assert.That(_service.GetBuffCount(BuffClassName.HealthBuff), Is.EqualTo(6));
    }

    [Test]
    public void HasBuff_MinCountCheck_WorksCorrectly()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.RageBuff, 2);
      Assert.That(_service.HasBuff(BuffClassName.RageBuff, 2), Is.True);
      Assert.That(_service.HasBuff(BuffClassName.RageBuff, 3), Is.False);
    }

    #endregion

    #region RemoveBuff

    [Test]
    public void RemoveBuff_WithSufficientItems_RemovesAndReturnsTrue()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.SpeedBuff, 5);
      bool result = _service.RemoveBuff(BuffClassName.SpeedBuff, 3);
      Assert.That(result, Is.True);
      Assert.That(_service.GetBuffCount(BuffClassName.SpeedBuff), Is.EqualTo(2));
    }

    [Test]
    public void RemoveBuff_InsufficientItems_ReturnsFalse()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.GodBuff, 1);
      bool result = _service.RemoveBuff(BuffClassName.GodBuff, 5);
      Assert.That(result, Is.False);
      Assert.That(_service.GetBuffCount(BuffClassName.GodBuff), Is.EqualTo(1)); // unchanged
    }

    [Test]
    public void RemoveBuff_NotPresent_ReturnsFalse()
    {
      bool result = _service.RemoveBuff(BuffClassName.DamageBuff, 1);
      Assert.That(result, Is.False);
    }

    [Test]
    public void RemoveBuff_ClearsSlotWhenCountReachesZero()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.DamageBuff, 1);
      _service.RemoveBuff(BuffClassName.DamageBuff, 1);
      Assert.That(_service.GetInventorySlot(0).IsEmpty, Is.True);
    }

    [Test]
    public void RemoveBuff_SpillsToHotbar_WhenInventoryInsufficient()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.SpeedBuff, 2);
      _service.GetHotbarSlot(0).Set(BuffClassName.SpeedBuff, 3);
      bool result = _service.RemoveBuff(BuffClassName.SpeedBuff, 4);
      Assert.That(result, Is.True);
      Assert.That(_service.GetBuffCount(BuffClassName.SpeedBuff), Is.EqualTo(1));
    }

    #endregion

    #region MoveOrSwapInventorySlots

    [Test]
    public void MoveOrSwapInventorySlots_ValidMove_SwapsContents()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.SpeedBuff, 2);
      _service.GetInventorySlot(1).Set(BuffClassName.HealthBuff, 3);

      bool result = _service.MoveOrSwapInventorySlots(0, 1);

      Assert.That(result, Is.True);
      Assert.That(_service.GetInventorySlot(0).BuffClass, Is.EqualTo(BuffClassName.HealthBuff));
      Assert.That(_service.GetInventorySlot(1).BuffClass, Is.EqualTo(BuffClassName.SpeedBuff));
    }

    [Test]
    public void MoveOrSwapInventorySlots_SameIndex_ReturnsFalse()
    {
      bool result = _service.MoveOrSwapInventorySlots(2, 2);
      Assert.That(result, Is.False);
    }

    [Test]
    public void MoveOrSwapInventorySlots_OutOfRange_ReturnsFalse()
    {
      bool result = _service.MoveOrSwapInventorySlots(-1, 0);
      Assert.That(result, Is.False);
    }

    #endregion

    #region MoveOrSwapHotbarSlots

    [Test]
    public void MoveOrSwapHotbarSlots_ValidMove_SwapsContents()
    {
      _service.GetHotbarSlot(0).Set(BuffClassName.GodBuff, 1);
      _service.GetHotbarSlot(1).Set(BuffClassName.RageBuff, 2);

      bool result = _service.MoveOrSwapHotbarSlots(0, 1);

      Assert.That(result, Is.True);
      Assert.That(_service.GetHotbarSlot(0).BuffClass, Is.EqualTo(BuffClassName.RageBuff));
      Assert.That(_service.GetHotbarSlot(1).BuffClass, Is.EqualTo(BuffClassName.GodBuff));
    }

    [Test]
    public void MoveOrSwapHotbarSlots_SameIndex_ReturnsFalse()
    {
      bool result = _service.MoveOrSwapHotbarSlots(1, 1);
      Assert.That(result, Is.False);
    }

    #endregion

    #region MoveInventoryToHotbar

    [Test]
    public void MoveInventoryToHotbar_ValidIndices_SwapsSlots()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.RegenBuff, 4);
      bool result = _service.MoveInventoryToHotbar(0, 0);
      Assert.That(result, Is.True);
      Assert.That(_service.GetHotbarSlot(0).BuffClass, Is.EqualTo(BuffClassName.RegenBuff));
      Assert.That(_service.GetInventorySlot(0).IsEmpty, Is.True);
    }

    [Test]
    public void MoveInventoryToHotbar_InvalidInventoryIndex_ReturnsFalse()
    {
      bool result = _service.MoveInventoryToHotbar(-1, 0);
      Assert.That(result, Is.False);
    }

    [Test]
    public void MoveInventoryToHotbar_InvalidHotbarIndex_ReturnsFalse()
    {
      bool result = _service.MoveInventoryToHotbar(0, HotbarSize + 5);
      Assert.That(result, Is.False);
    }

    #endregion

    #region MoveHotbarToInventory

    [Test]
    public void MoveHotbarToInventory_ValidIndices_SwapsSlots()
    {
      _service.GetHotbarSlot(1).Set(BuffClassName.DamageBuff, 2);
      bool result = _service.MoveHotbarToInventory(1, 3);
      Assert.That(result, Is.True);
      Assert.That(_service.GetInventorySlot(3).BuffClass, Is.EqualTo(BuffClassName.DamageBuff));
    }

    #endregion

    #region SelectHotbarSlot

    [Test]
    public void SelectHotbarSlot_ValidIndex_UpdatesSelection()
    {
      _service.SelectHotbarSlot(2);
      Assert.That(_service.SelectedHotbarIndex, Is.EqualTo(2));
    }

    [Test]
    public void SelectHotbarSlot_InvalidIndex_DoesNotChange()
    {
      _service.SelectHotbarSlot(0);
      _service.SelectHotbarSlot(HotbarSize + 5);
      Assert.That(_service.SelectedHotbarIndex, Is.EqualTo(0));
    }

    [Test]
    public void SelectHotbarSlot_SameIndex_CanFireEvent()
    {
      bool fired = false;
      _service.OnHotbarSelectionChanged
        .Subscribe(_ => fired = true);
      _service.SelectHotbarSlot(0); // already 0
      Assert.That(fired, Is.True);
    }

    [Test]
    public void SelectHotbarSlot_FiresSelectionChangedEvent()
    {
      int received = -1;
      _service.OnHotbarSelectionChanged
        .Subscribe(idx => received = idx);
      _service.SelectHotbarSlot(3);
      Assert.That(received, Is.EqualTo(3));
    }

    #endregion

    #region GetAllSlots

    [Test]
    public void GetAllInventorySlots_ReturnsCorrectCount()
    {
      Assert.That(_service.GetAllInventorySlots(), Has.Count.EqualTo(InventorySize));
    }

    [Test]
    public void GetAllHotbarSlots_ReturnsCorrectCount()
    {
      Assert.That(_service.GetAllHotbarSlots(), Has.Count.EqualTo(HotbarSize));
    }

    [Test]
    public void GetAllInventorySlots_ReturnsNewList_NotSameReference()
    {
      var list1 = _service.GetAllInventorySlots();
      var list2 = _service.GetAllInventorySlots();
      Assert.That(list1, Is.Not.SameAs(list2));
    }

    #endregion

    #region ClearInventory / ClearHotbar / ClearAll

    [Test]
    public void ClearInventory_RemovesAllItems()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.SpeedBuff, 3);
      _service.GetInventorySlot(5).Set(BuffClassName.HealthBuff, 1);
      _service.ClearInventory();

      for (int i = 0; i < InventorySize; i++)
        Assert.That(_service.GetInventorySlot(i).IsEmpty, Is.True);
    }

    [Test]
    public void ClearHotbar_RemovesAllItems()
    {
      _service.GetHotbarSlot(0).Set(BuffClassName.RageBuff, 2);
      _service.ClearHotbar();

      for (int i = 0; i < HotbarSize; i++)
        Assert.That(_service.GetHotbarSlot(i).IsEmpty, Is.True);
    }

    [Test]
    public void ClearAll_ResetsEverythingIncludingSelection()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.GodBuff, 1);
      _service.GetHotbarSlot(0).Set(BuffClassName.DamageBuff, 1);
      _service.SelectHotbarSlot(2);
      _service.ClearAll();

      Assert.That(_service.GetBuffCount(BuffClassName.GodBuff), Is.EqualTo(0));
      Assert.That(_service.GetBuffCount(BuffClassName.DamageBuff), Is.EqualTo(0));
      Assert.That(_service.SelectedHotbarIndex, Is.EqualTo(0));
    }

    #endregion

    #region GetSaveData / LoadFromSaveData

    [Test]
    public void GetSaveData_ReflectsCurrentState()
    {
      _service.GetInventorySlot(0).Set(BuffClassName.SpeedBuff, 3);
      _service.GetHotbarSlot(1).Set(BuffClassName.HealthBuff, 2);
      _service.SelectHotbarSlot(1);

      var saveData = _service.GetSaveData();

      Assert.That(saveData.InventorySlots[0].BuffClass, Is.EqualTo(BuffClassName.SpeedBuff));
      Assert.That(saveData.HotbarSlots[1].BuffClass, Is.EqualTo(BuffClassName.HealthBuff));
      Assert.That(saveData.SelectedHotbarIndex, Is.EqualTo(1));
    }

    [Test]
    public void LoadFromSaveData_RestoresInventoryContents()
    {
      var saveData = new InventorySaveData();
      saveData.InitializeSlots(InventorySize, HotbarSize);
      saveData.InventorySlots[2].Set(BuffClassName.RageBuff, 4);
      saveData.SelectedHotbarIndex = 2;

      _service.LoadFromSaveData(saveData);

      Assert.That(_service.GetInventorySlot(2).BuffClass, Is.EqualTo(BuffClassName.RageBuff));
      Assert.That(_service.SelectedHotbarIndex, Is.EqualTo(2));
    }

    [Test]
    public void LoadFromSaveData_NullSaveData_DoesNotThrow()
    {
      Assert.DoesNotThrow(() => _service.LoadFromSaveData(null));
    }

    [Test]
    public void Events_InventorySlotChanged_FiredOnClear()
    {
      var changed = new List<int>();
      _service.OnInventorySlotChanged
        .Subscribe(i => changed.Add(i));
      _service.GetInventorySlot(3).Set(BuffClassName.DamageBuff, 1);
      _service.ClearInventory();
      Assert.That(changed, Contains.Item(3));
    }

    #endregion
  }
}
