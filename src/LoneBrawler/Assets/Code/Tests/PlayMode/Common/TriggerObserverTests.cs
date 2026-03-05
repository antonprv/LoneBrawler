// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.UtilityComponents;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Common
{
  /// <summary>
  /// PlayMode tests for TriggerObserver.
  /// Use real Unity physics (Rigidbody + Collider) to verify
  /// ObservedOnTriggerEnter/Exit events.
  /// </summary>
  public class TriggerObserverTests
  {
    private GameObject _observerGo;
    private GameObject _enteringGo;
    private TriggerObserver _observer;

    [SetUp]
    public void SetUp()
    {
      // Main object with TriggerObserver - large trigger cube
      _observerGo = new GameObject("Observer");
      _observerGo.transform.position = Vector3.zero;

      var col = _observerGo.AddComponent<BoxCollider>();
      col.size = new Vector3(4, 4, 4);
      col.isTrigger = true;

      _observer = _observerGo.AddComponent<TriggerObserver>();

      // Object that will fly into the trigger
      _enteringGo = new GameObject("Entering");
      _enteringGo.transform.position = new Vector3(10, 0, 0); // starts outside

      var rb = _enteringGo.AddComponent<Rigidbody>();
      rb.useGravity = false;
      rb.isKinematic = true;

      var entCol = _enteringGo.AddComponent<BoxCollider>();
      entCol.size = Vector3.one;
    }

    [TearDown]
    public void TearDown()
    {
      Object.Destroy(_observerGo);
      Object.Destroy(_enteringGo);
    }

    #region Subscribe To Events

    [UnityTest]
    public IEnumerator ObservedOnTriggerEnter_Fires_WhenObjectEntersTrigger()
    {
      Collider received = null;
      _observer.ObservedOnTriggerEnter += col => received = col;

      // Move object inside the trigger
      _enteringGo.transform.position = Vector3.zero;

      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      Assert.That(received, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator ObservedOnTriggerExit_Fires_WhenObjectLeavesTrigger()
    {
      bool exitFired = false;
      _observer.ObservedOnTriggerExit += _ => exitFired = true;

      // First place object inside
      _enteringGo.transform.position = Vector3.zero;
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      // Then move outside
      _enteringGo.transform.position = new Vector3(10, 0, 0);
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      Assert.That(exitFired, Is.True);
    }

    [UnityTest]
    public IEnumerator ObservedOnTriggerEnter_PassesCorrectCollider()
    {
      Collider received = null;
      _observer.ObservedOnTriggerEnter += col => received = col;

      _enteringGo.transform.position = Vector3.zero;
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      Assert.That(received, Is.Not.Null);
      Assert.That(received.gameObject, Is.EqualTo(_enteringGo));
    }

    [UnityTest]
    public IEnumerator NoSubscribers_TriggerEnter_DoesNotThrow()
    {
      // Without subscribers - event should be ignored without exceptions
      _enteringGo.transform.position = Vector3.zero;

      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      // If we've got here - everything is fine
      Assert.Pass("No exception thrown");
    }

    [UnityTest]
    public IEnumerator MultipleSubscribers_AllReceiveEvent()
    {
      int count = 0;
      _observer.ObservedOnTriggerEnter += _ => count++;
      _observer.ObservedOnTriggerEnter += _ => count++;

      _enteringGo.transform.position = Vector3.zero;
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      Assert.That(count, Is.EqualTo(2));
    }

    #endregion
  }
}
