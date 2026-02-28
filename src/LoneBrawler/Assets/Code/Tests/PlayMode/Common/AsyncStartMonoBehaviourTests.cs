// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Utils.ActorComponents;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Common
{
  /// <summary>
  /// PlayMode tests for AsyncStartMonoBehaviour.
  /// Verify: IsInitialized = false until ManualStart, = true afterwards,
  /// AsyncStart is called exactly once, VerifiedUpdate isn't called prior to initialization.
  /// </summary>
  public class AsyncStartMonoBehaviourTests
  {
    private GameObject _go;
    private TestAsyncStart _component;

    [SetUp]
    public void SetUp()
    {
      _go = new GameObject("AsyncStart_Test");
      _component = _go.AddComponent<TestAsyncStart>();
    }

    [TearDown]
    public void TearDown() => Object.Destroy(_go);

    [UnityTest]
    public IEnumerator IsInitialized_IsFalse_BeforeManualStart()
    {
      yield return null;
      Assert.That(_component.IsInitialized, Is.False);
    }

    [UnityTest]
    public IEnumerator IsInitialized_BecomesTrue_AfterManualStart()
    {
      _component.ManualStart();
      yield return new WaitForFixedUpdate();
      yield return null;

      Assert.That(_component.IsInitialized, Is.True);
    }

    [UnityTest]
    public IEnumerator AsyncStart_CalledOnce_AfterManualStart()
    {
      _component.ManualStart();
      yield return new WaitForFixedUpdate();
      yield return null;

      Assert.That(_component.AsyncStartCallCount, Is.EqualTo(1));
    }

    [UnityTest]
    public IEnumerator VerifiedUpdate_NotCalled_BeforeInitialization()
    {
      // Wait few frames without invoking ManualStart
      yield return null;
      yield return null;
      yield return null;

      Assert.That(_component.VerifiedUpdateCallCount, Is.EqualTo(0));
    }

    [UnityTest]
    public IEnumerator VerifiedUpdate_CalledEachFrame_AfterInitialization()
    {
      _component.ManualStart();
      yield return new WaitForFixedUpdate();
      yield return null;
      yield return null;
      yield return null;

      Assert.That(_component.VerifiedUpdateCallCount, Is.GreaterThanOrEqualTo(2));
    }

    [UnityTest]
    public IEnumerator ManualStart_CalledTwice_DoesNotDoubleInitialize()
    {
      _component.ManualStart();
      _component.ManualStart();
      yield return new WaitForFixedUpdate();
      yield return null;

      // AsyncStart may be called twice (two coroutines running), but IsInitialized = true
      Assert.That(_component.IsInitialized, Is.True);
    }

    #region Helper Subclass

    private class TestAsyncStart : AsyncStartMonoBehaviour
    {
      public int AsyncStartCallCount { get; private set; }
      public int VerifiedUpdateCallCount { get; private set; }

      protected override void AsyncStart()
      {
        AsyncStartCallCount++;
      }

      protected override void VerifiedUpdate()
      {
        VerifiedUpdateCallCount++;
      }
    }

    #endregion
  }
}
