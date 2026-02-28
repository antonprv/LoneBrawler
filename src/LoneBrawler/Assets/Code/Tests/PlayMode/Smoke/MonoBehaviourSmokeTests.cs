// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.UtilityComponents;
using Code.Gameplay.Features.Enemies.Health;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Save;
using Code.Gameplay.Utils.ActorComponents;

using NUnit.Framework;

using Tests.PlayMode.Common;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Smoke
{
  /// <summary>
  /// Smoke tests — check that MonoBehaviour components can be created
  /// on a GameObject without exceptions or dependencies.
  /// These are the 'gateways' before more detailed tests.
  /// </summary>
  public class MonoBehaviourSmokeTests
  {
    [UnityTest]
    public IEnumerator PlayerHealth_CanBeAdded_ToGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // PlayerHealth has [RequireComponent(typeof(PlayerAnimator))].
      // PlayerAnimator needs CharacterController and ITimeService (injected via Zenjex).
      // CreatePlayerGameObject satisfies all of these upfront.
      var go = ZenjexTestBootstrap.CreatePlayerGameObject();
      Assert.DoesNotThrow(() => go.AddComponent<PlayerHealth>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator PlayerDeath_CanBeAdded_ToGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // PlayerDeath has [RequireComponent(typeof(PlayerAnimator))] and
      // [RequireComponent(typeof(PlayerMove))].
      // CreatePlayerGameObject adds both along with CharacterController.
      var go = ZenjexTestBootstrap.CreatePlayerGameObject();
      Assert.DoesNotThrow(() => go.AddComponent<PlayerDeath>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator EnemyHealth_CanBeAdded_ToGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject();
      Assert.DoesNotThrow(() => go.AddComponent<EnemyHealth>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator EnemyDeath_CanBeAdded_ToGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject();
      Assert.DoesNotThrow(() => go.AddComponent<EnemyDeath>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator SaveComponent_CanBeAdded_ToGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject();
      Assert.DoesNotThrow(() => go.AddComponent<SaveComponent>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator TriggerObserver_CanBeAdded_RequiresCollider()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject();
      go.AddComponent<BoxCollider>(); // TriggerObserver требует Collider
      Assert.DoesNotThrow(() => go.AddComponent<TriggerObserver>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator UniqueId_CanBeAdded_ToGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject();
      Assert.DoesNotThrow(() => go.AddComponent<Code.Gameplay.Utils.UniqueId>());
      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator AsyncStartMonoBehaviour_IsNotInitialized_ByDefault()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject();
      var comp = go.AddComponent<AsyncStartMonoBehaviour>();
      yield return null;

      Assert.That(comp.IsInitialized, Is.False);
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    [UnityTest]
    public IEnumerator AllCriticalComponents_CanBeAddedToSingleGameObject()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // CreatePlayerGameObject is used because PlayerHealth and PlayerDeath
      // both require PlayerAnimator (and PlayerDeath also requires PlayerMove),
      // which in turn needs CharacterController to tick without exceptions.
      var go = ZenjexTestBootstrap.CreatePlayerGameObject();
      go.AddComponent<BoxCollider>();

      Assert.DoesNotThrow(() =>
      {
        go.AddComponent<PlayerHealth>();
        go.AddComponent<PlayerDeath>();
        go.AddComponent<SaveComponent>();
        go.AddComponent<TriggerObserver>();
      });

      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }
  }
}
