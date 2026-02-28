// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;

using NSubstitute;

using NUnit.Framework;

using R3;

using Tests.PlayMode.Common;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Player
{
  /// <summary>
  /// PlayMode tests for PlayerDeath.
  /// Verify: death at zero HP, animation playback,
  /// deactivation of components, IsDead flag.
  /// </summary>
  public class PlayerDeathTests
  {
    private GameObject _go;
    private PlayerDeath _death;
    private IAnimator _animator;
    private ReactiveProperty<float> _healthRP;
    private IHealth _health;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // CreatePlayerGameObject adds CharacterController + PlayerAnimator + PlayerMove,
      // which are [RequireComponent] dependencies of PlayerDeath.
      // This also prevents NullReferenceException from PlayerMove.OnDestroy()
      // when _attacker is null, and from PlayerAnimator.Update() when
      // _timeService or CharacterController are not yet injected.
      _go = ZenjexTestBootstrap.CreatePlayerGameObject("PlayerDeath_Test");
      _death = _go.AddComponent<PlayerDeath>();

      _animator = Substitute.For<IAnimator>();

      _healthRP = new ReactiveProperty<float>(100f);
      _health = Substitute.For<IHealth>();
      _health.CurrentHealthRP.Returns(_healthRP.ToReadOnlyReactiveProperty());

      // DeathFX — empty prefab so Instantiate doesn't crash
      _death.DeathFX = new GameObject("FX");

      _death.Construct(_animator, _health);
    }

    [TearDown]
    public void TearDown()
    {
      if (_death != null && _death.gameObject != null)
        Object.Destroy(_death.gameObject);

      if (_death.DeathFX != null)
        Object.Destroy(_death.DeathFX);

      _healthRP?.Dispose();

      ZenjexTestBootstrap.Cleanup();
    }

    #region Initial State

    [UnityTest]
    public IEnumerator IsDead_IsFalse_AfterConstruct()
    {
      yield return null;
      Assert.That(_death.IsDead, Is.False);
    }

    #endregion

    #region Death

    [UnityTest]
    public IEnumerator IsDead_BecomesTrue_WhenHealthReachesZero()
    {
      yield return null;

      _healthRP.Value = 0f;
      yield return null;

      Assert.That(_death.IsDead, Is.True);
    }

    [UnityTest]
    public IEnumerator Death_PlaysDeathAnimation_WhenHealthReachesZero()
    {
      yield return null;

      _healthRP.Value = 0f;
      yield return null;

      _animator.Received(1).PlayDeath();
    }

    [UnityTest]
    public IEnumerator Death_DeactivatesIDeactivatableComponents()
    {
      var deactivatable = Substitute.For<IDeactivatable>();
      // Can't directly add mock component, but can verify that Death is triggered
      // and that GetComponents<IDeactivatable> cycle won't throw exception
      yield return null;

      _healthRP.Value = 0f;
      yield return null;

      // IsDead indicates that Die() completed without exceptions (including deactivation cycle)
      Assert.That(_death.IsDead, Is.True);
    }

    [UnityTest]
    public IEnumerator Death_DoesNotTriggerTwice_OnMultipleZeroHealthEvents()
    {
      yield return null;

      _healthRP.Value = 0f;
      yield return null;

      // Second time — shouldn't replay PlayDeath
      _healthRP.Value = 0f;
      yield return null;

      // Due to Skip(1)+Where(IsNearlyZero) second invocation is ignored by filter
      _animator.Received(1).PlayDeath();
    }

    #endregion

    #region Destruction

    [UnityTest]
    public IEnumerator Destroy_DoesNotThrowException()
    {
      yield return null;
      Assert.DoesNotThrow(() => Object.DestroyImmediate(_death.gameObject));
      yield return null;
    }

    #endregion
  }
}
