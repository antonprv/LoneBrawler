// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Tests.PlayMode.Helpers;

using NSubstitute;

using NUnit.Framework;

using R3;

using Tests.PlayMode.Common;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Player
{
  /// <summary>
  /// Integration PlayMode tests for PlayerHealth.
  /// Check: damage, healing, invulnerability, reactive properties,
  /// correctness of operation on real MonoBehaviour in game loop.
  /// </summary>
  public class PlayerHealthTests
  {
    private GameObject _go;
    private PlayerHealth _health;
    private IAnimator _animator;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // CreatePlayerGameObject adds CharacterController + PlayerAnimator + PlayerMove,
      // satisfying all [RequireComponent] constraints and preventing NullReferenceException
      // in PlayerAnimator.Update() and PlayerMove.Update() during the test game loop.
      _go = ZenjexTestBootstrap.CreatePlayerGameObject("PlayerHealth_Test");
      _health = _go.AddComponent<PlayerHealth>();
      _animator = Substitute.For<IAnimator>();
      _health.Construct(_animator);
    }

    [TearDown]
    public void TearDown()
    {
      Object.Destroy(_go);
      ZenjexTestBootstrap.Cleanup();
    }

    #region Initialization

    [UnityTest]
    public IEnumerator InitialHealth_IsZero_AfterAddingComponent()
    {
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(0f));
      Assert.That(_health.MaxHealthRP.CurrentValue, Is.EqualTo(0f));
    }

    [UnityTest]
    public IEnumerator ReadProgress_SetsCurrentAndMaxHealth_Correctly()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 70f);
      _health.ReadProgress(progress);

      yield return null;

      Assert.That(_health.MaxHealthRP.CurrentValue, Is.EqualTo(100f));
      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(70f));
    }

    #endregion

    #region Damage

    [UnityTest]
    public IEnumerator TakeDamage_ReducesCurrentHealth()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.TakeDamage(30f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(70f));
    }

    [UnityTest]
    public IEnumerator TakeDamage_PlaysHitAnimation()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.TakeDamage(10f);
      yield return null;

      _animator.Received(1).PlayHit();
    }

    [UnityTest]
    public IEnumerator TakeDamage_WhenHealthIsZero_DoesNotGoNegative()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 0f);
      _health.ReadProgress(progress);

      _health.TakeDamage(50f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.GreaterThanOrEqualTo(0f));
      _animator.DidNotReceive().PlayHit();
    }

    [UnityTest]
    public IEnumerator TakeDamage_WhenInvulnerable_DoesNotReduceHealth()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.SetInvulnerable(true);
      _health.TakeDamage(50f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(100f));
      _animator.DidNotReceive().PlayHit();
    }

    [UnityTest]
    public IEnumerator TakeDamage_AfterRemovingInvulnerability_ReducesHealth()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.SetInvulnerable(true);
      _health.SetInvulnerable(false);
      _health.TakeDamage(30f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(70f));
    }

    #endregion

    #region Damage Modifier

    [UnityTest]
    public IEnumerator ApplyDamageModifier_HalvesDamage()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.ApplyDamageModifier(0.5f);
      _health.TakeDamage(40f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(80f));
    }

    [UnityTest]
    public IEnumerator RemoveDamageModifier_RestoresFullDamage()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.ApplyDamageModifier(0.5f);
      _health.RemoveDamageModifier(0.5f);
      _health.TakeDamage(40f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(60f));
    }

    #endregion

    #region Healing

    [UnityTest]
    public IEnumerator Heal_IncreasesCurrentHealth()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 50f);
      _health.ReadProgress(progress);

      _health.Heal(20f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(70f));
    }

    [UnityTest]
    public IEnumerator Heal_DoesNotExceedMaxHealth()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 90f);
      _health.ReadProgress(progress);

      _health.Heal(50f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(100f));
    }

    #endregion

    #region AddMaxHealth

    [UnityTest]
    public IEnumerator AddMaxHealth_IncreasesMaxHealthValue()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      _health.AddMaxHealth(50f);
      yield return null;

      Assert.That(_health.MaxHealthRP.CurrentValue, Is.EqualTo(150f));
    }

    #endregion

    #region ReactiveProperty

    [UnityTest]
    public IEnumerator CurrentHealthRP_EmitsEvent_WhenDamageTaken()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      float receivedValue = -1f;
      _health.CurrentHealthRP
        .Skip(1)
        .Subscribe(hp => receivedValue = hp);

      _health.TakeDamage(25f);
      yield return null;

      Assert.That(receivedValue, Is.EqualTo(75f));
    }

    #endregion

    #region WriteToProgress

    [UnityTest]
    public IEnumerator WriteToProgress_SavesCurrentState()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);
      _health.TakeDamage(40f);

      var saved = TestHelpers.CreateProgress(maxHealth: 0f, currentHealth: 0f);
      _health.WriteToProgress(saved);
      yield return null;

      Assert.That(saved.PLayerState.CurrentHealth, Is.EqualTo(60f));
      Assert.That(saved.PLayerState.MaxHealth, Is.EqualTo(100f));
    }

    #endregion

    #region Object Destruction

    [UnityTest]
    public IEnumerator Destroy_DoesNotThrow()
    {
      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      _health.ReadProgress(progress);

      Assert.DoesNotThrow(() => Object.DestroyImmediate(_go));
      yield return null;
    }

    #endregion
  }
}
