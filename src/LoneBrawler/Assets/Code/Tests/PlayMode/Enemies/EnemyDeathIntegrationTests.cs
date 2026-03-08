// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Attack;
using Code.Data.StaticData.Types.Enemies;
using Code.Gameplay.Features.Enemies.Health;
using Code.Gameplay.Utils.NPCInterfaces.Animations;

using NSubstitute;

using NUnit.Framework;

using R3;

using Tests.PlayMode.Common;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Common
{
  /// <summary>
  /// Integration PlayMode tests for EnemyHealth → EnemyDeath chain.
  /// Simulate enemy life cycle on real MonoBehaviour objects.
  /// </summary>
  public class EnemyDeathIntegrationTests
  {
    private GameObject _enemyGo;
    private EnemyHealth _health;
    private EnemyDeath _death;
    private IAnimator _animator;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
      yield return ZenjexTestBootstrap.Initialize();

      _enemyGo = new GameObject("Enemy_Integration");

      _animator = Substitute.For<IAnimator>();

      _health = _enemyGo.AddComponent<EnemyHealth>();
      _health.Construct(_animator);

      _death = _enemyGo.AddComponent<EnemyDeath>();

      var staticData = ScriptableObject.CreateInstance<EnemyStaticData>();
      staticData.MaxHealth = 100f;
      staticData.AttackCooldown = 1f;
      staticData.DisappearDelay = 0.05f;
      staticData.EnemyTypeId = EnemyTypeId.Lich;
      staticData.EnemyAttackType = EnemyAttackType.Melee;

      _health.SetValues(staticData);
      _death.SetValues(staticData);
      _death.Construct(_animator, _health);
    }

    [TearDown]
    public void TearDown()
    {
      if (_death != null)
        Object.Destroy(_death);

      // _enemyGo might have been destroyed in the test
      if (_enemyGo != null)
        Object.Destroy(_enemyGo);

      ZenjexTestBootstrap.Cleanup();
    }

    #region Enemy Lifecycle

    [UnityTest]
    public IEnumerator EnemyStartsAlive_WithFullHealth()
    {
      yield return null;

      Assert.That(_death.IsDead, Is.False);
      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(100f));
    }

    #endregion

    #region Partial Damage

    [UnityTest]
    public IEnumerator AfterPartialDamage_EnemyIsAlive()
    {
      _health.TakeDamage(50f);
      yield return null;

      Assert.That(_death.IsDead, Is.False);
      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(50f));
    }

    #endregion

    #region Lethal Damage

    [UnityTest]
    public IEnumerator AfterLethalDamage_EnemyIsDead()
    {
      _health.TakeDamage(100f);
      yield return null;
      yield return null;

      Assert.That(_death.IsDead, Is.True);
    }

    [UnityTest]
    public IEnumerator AfterLethalDamage_PlaysDeath_Animation()
    {
      _health.TakeDamage(100f);
      yield return null;
      yield return null;

      _animator.Received(1).PlayDeath();
    }

    [UnityTest]
    public IEnumerator OnDead_Event_FiresWhenEnemyDies()
    {
      bool fired = false;
      _death.OnDead.Subscribe(_ => fired = true);

      _health.TakeDamage(100f);
      yield return null;
      yield return null;

      Assert.That(fired, Is.True);
    }

    #endregion

    #region DisappearDelay → Auto Destruction
    [UnityTest]
    public IEnumerator AfterDisappearDelay_EnemyGameObjectIsDestroyed()
    {
      _health.TakeDamage(100f);
      yield return null;

      yield return new WaitForSeconds(0.1f); // > DisappearDelay (0.05f)

      Assert.That(_enemyGo == null || !_enemyGo.activeSelf, Is.True);
      _enemyGo = null; // avoid duplicate Destroy in TearDown
    }

    #endregion

    #region End Of ReactiveProperty Lifecycle

    [UnityTest]
    public IEnumerator MultipleSmallDamages_TriggerDeath_WhenHealthHitsZero()
    {
      for (int i = 0; i < 10; i++)
        _health.TakeDamage(10f);

      yield return null;
      yield return null;

      Assert.That(_death.IsDead, Is.True);
    }

    #endregion
  }
}
