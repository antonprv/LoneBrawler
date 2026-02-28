// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.UtilityComponents;
using Code.Gameplay.Features.Enemies.Aggro;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;

using NSubstitute;

using NUnit.Framework;

using Tests.PlayMode.Common;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Enemies
{
  /// <summary>
  /// PlayMode tests for Aggro + TriggerObserver.
  /// Verify: aggression on trigger entry, stop on exit (with delay),
  /// activation/deactivation via Activate/Deactivate, correct OnDestroy behavior.
  /// </summary>
  public class AggroTests
  {
    private GameObject _aggroGo;
    private GameObject _triggerGo;
    private GameObject _playerGo;

    private Aggro _aggro;
    private TriggerObserver _triggerObserver;
    private IMovableAgent _movableAgent;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // Aggro object with agent (enemy)
      _aggroGo = new GameObject("Aggro_Test");
      _aggro = _aggroGo.AddComponent<Aggro>();
      _aggro.followDelay = 0.05f; // minimum delay for testing purposes

      // TriggerObserver — aggro zone
      _triggerGo = new GameObject("Trigger");
      _triggerGo.transform.SetParent(_aggroGo.transform);
      _triggerGo.transform.localPosition = Vector3.zero;

      var col = _triggerGo.AddComponent<BoxCollider>();
      col.size = new Vector3(6, 6, 6);
      col.isTrigger = true;

      _triggerObserver = _triggerGo.AddComponent<TriggerObserver>();
      _aggro.triggerObserver = _triggerObserver;

      // Mock moving agent
      _movableAgent = Substitute.For<IMovableAgent>();
      _aggro.Construct(_movableAgent);

      // Player
      _playerGo = new GameObject("Player");
      _playerGo.transform.position = new Vector3(20, 0, 0); // outside
      var rb = _playerGo.AddComponent<Rigidbody>();
      rb.useGravity = false;
      rb.isKinematic = true;
      _playerGo.AddComponent<BoxCollider>();

      // Manual start (AsyncStartMonoBehaviour)
      _aggro.ManualStart();
    }

    [TearDown]
    public void TearDown()
    {
      Object.Destroy(_aggroGo);
      Object.Destroy(_playerGo);
      ZenjexTestBootstrap.Cleanup();
    }

    #region Initialization

    [UnityTest]
    public IEnumerator AfterManualStart_AgentStopsFollowing_Immediately()
    {
      yield return new WaitForFixedUpdate();
      yield return null;

      _movableAgent.Received(1).StopFollowingImmediately();
    }

    #endregion

    #region Trigger Enter

    [UnityTest]
    public IEnumerator PlayerEntersTrigger_AgentStartsFollowing()
    {
      yield return new WaitForFixedUpdate();
      yield return null;

      _movableAgent.ClearReceivedCalls();

      _playerGo.transform.position = Vector3.zero;
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      _movableAgent.Received(1).ContinueFollowing();
    }

    #endregion

    #region Trigger Exit (Delayed)

    [UnityTest]
    public IEnumerator PlayerExitsTrigger_AgentStopsFollowing_AfterDelay()
    {
      yield return new WaitForFixedUpdate();
      yield return null;

      // Player enters trigger
      _playerGo.transform.position = Vector3.zero;
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      _movableAgent.ClearReceivedCalls();

      // Player exits trigger
      _playerGo.transform.position = new Vector3(20, 0, 0);
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();

      // Shouldn't stop yet — waiting for delay
      _movableAgent.DidNotReceive().StopFollowingImmediately();

      yield return new WaitForSeconds(0.1f); // greater than followDelay (0.05f)

      _movableAgent.Received(1).StopFollowingImmediately();
    }

    #endregion

    #region Activate / Deactivate

    [UnityTest]
    public IEnumerator Deactivate_DisablesComponent()
    {
      yield return new WaitForFixedUpdate();
      yield return null;

      _aggro.Deactivate();
      yield return null;

      Assert.That(_aggro.enabled, Is.False);
    }

    [UnityTest]
    public IEnumerator Activate_EnablesComponent()
    {
      yield return new WaitForFixedUpdate();
      yield return null;

      _aggro.Deactivate();
      _aggro.Activate();
      yield return null;

      Assert.That(_aggro.enabled, Is.True);
    }

    #endregion

    #region Destruction

    [UnityTest]
    public IEnumerator Destroy_AfterInit_DoesNotThrow()
    {
      yield return new WaitForFixedUpdate();
      yield return null;

      Assert.DoesNotThrow(() => Object.DestroyImmediate(_aggroGo));
      yield return null;
    }

    #endregion
  }
}
