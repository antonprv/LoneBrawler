// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DragDropService.Types;

using Code.Data.SaveData.Inventory;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.Services.DragDropService;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.EditMode.Services
{
  [TestFixture]
  public class DragDropServiceTests
  {
    private DragDropService _service;

    [SetUp]
    public void SetUp() => _service = new DragDropService();

    #region StartDrag

    [Test]
    public void StartDrag_ValidData_SetsDraggingState()
    {
      _service.StartDrag(BuffClassName.HealthBuff, 3, DragSource.Inventory, 0);
      Assert.That(_service.IsDragging, Is.True);
      Assert.That(_service.DraggedBuffClass, Is.EqualTo(BuffClassName.HealthBuff));
      Assert.That(_service.DraggedCount, Is.EqualTo(3));
      Assert.That(_service.Source, Is.EqualTo(DragSource.Inventory));
      Assert.That(_service.SourceIndex, Is.EqualTo(0));
    }

    [Test]
    public void StartDrag_NoneBuffClass_DoesNotStartDrag()
    {
      LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Cannot start drag"));
      _service.StartDrag(BuffClassName.None, 1, DragSource.Inventory, 0);
      Assert.That(_service.IsDragging, Is.False);
    }

    [Test]
    public void StartDrag_ZeroCount_DoesNotStartDrag()
    {
      LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Cannot start drag"));
      _service.StartDrag(BuffClassName.HealthBuff, 0, DragSource.Inventory, 0);
      Assert.That(_service.IsDragging, Is.False);
    }

    [Test]
    public void StartDrag_NegativeCount_DoesNotStartDrag()
    {
      LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Cannot start drag"));
      _service.StartDrag(BuffClassName.HealthBuff, -1, DragSource.Inventory, 0);
      Assert.That(_service.IsDragging, Is.False);
    }

    [Test]
    public void StartDrag_FiresOnDragStartedEvent()
    {
      bool fired = false;
      _service.OnDragStarted += () => fired = true;
      _service.StartDrag(BuffClassName.SpeedBuff, 1, DragSource.Hotbar, 2);
      Assert.That(fired, Is.True);
    }

    #endregion

    #region EndDrag

    [Test]
    public void EndDrag_AfterStartDrag_ClearsState()
    {
      _service.StartDrag(BuffClassName.HealthBuff, 1, DragSource.Inventory, 0);
      _service.EndDrag();

      Assert.That(_service.IsDragging, Is.False);
      Assert.That(_service.DraggedBuffClass, Is.EqualTo(BuffClassName.None));
      Assert.That(_service.DraggedCount, Is.EqualTo(0));
      Assert.That(_service.SourceIndex, Is.EqualTo(-1));
    }

    [Test]
    public void EndDrag_WhenNotDragging_DoesNothing()
    {
      bool fired = false;
      _service.OnDragEnded += () => fired = true;
      _service.EndDrag(); // not dragging
      Assert.That(fired, Is.False);
    }

    [Test]
    public void EndDrag_FiresOnDragEndedEvent()
    {
      bool fired = false;
      _service.OnDragEnded += () => fired = true;
      _service.StartDrag(BuffClassName.RageBuff, 1, DragSource.Inventory, 0);
      _service.EndDrag();
      Assert.That(fired, Is.True);
    }

    #endregion

    #region CancelDrag

    [Test]
    public void CancelDrag_WhenDragging_ClearsState()
    {
      _service.StartDrag(BuffClassName.GodBuff, 2, DragSource.Hotbar, 1);
      _service.CancelDrag();
      Assert.That(_service.IsDragging, Is.False);
    }

    [Test]
    public void CancelDrag_WhenNotDragging_DoesNothing()
    {
      Assert.DoesNotThrow(() => _service.CancelDrag());
    }

    #endregion

    #region TryMergeOrSwap

    [Test]
    public void TryMergeOrSwap_EmptySlot_MovesAll()
    {
      _service.StartDrag(BuffClassName.HealthBuff, 5, DragSource.Inventory, 0);
      var target = new InventorySlotData();

      bool result = _service.TryMergeOrSwap(target, 10, out int remaining);

      Assert.That(result, Is.True);
      Assert.That(remaining, Is.EqualTo(0));
      Assert.That(target.BuffClass, Is.EqualTo(BuffClassName.HealthBuff));
      Assert.That(target.Count, Is.EqualTo(5));
    }

    [Test]
    public void TryMergeOrSwap_SameBuff_MergesUpToMaxStack()
    {
      _service.StartDrag(BuffClassName.SpeedBuff, 4, DragSource.Inventory, 0);
      var target = new InventorySlotData(BuffClassName.SpeedBuff, 7);

      bool result = _service.TryMergeOrSwap(target, 10, out int remaining);

      Assert.That(result, Is.True);
      Assert.That(target.Count, Is.EqualTo(10));    // 7 + 3 (only 3 fit)
      Assert.That(remaining, Is.EqualTo(1));         // 4 - 3 = 1 did not fit
    }

    [Test]
    public void TryMergeOrSwap_SameBuff_FullStack_NoChange()
    {
      _service.StartDrag(BuffClassName.DamageBuff, 3, DragSource.Inventory, 0);
      var target = new InventorySlotData(BuffClassName.DamageBuff, 10);

      bool result = _service.TryMergeOrSwap(target, 10, out int remaining);

      Assert.That(result, Is.True);
      Assert.That(remaining, Is.EqualTo(3));  // stack is full, nothing added
    }

    [Test]
    public void TryMergeOrSwap_DifferentBuff_ReturnsFalse()
    {
      _service.StartDrag(BuffClassName.SpeedBuff, 2, DragSource.Inventory, 0);
      var target = new InventorySlotData(BuffClassName.HealthBuff, 3);

      bool result = _service.TryMergeOrSwap(target, 10, out int remaining);

      Assert.That(result, Is.False);
    }

    [Test]
    public void TryMergeOrSwap_WhenNotDragging_ReturnsFalse()
    {
      var target = new InventorySlotData();
      bool result = _service.TryMergeOrSwap(target, 10, out int remaining);
      Assert.That(result, Is.False);
    }

    #endregion
  }
}
