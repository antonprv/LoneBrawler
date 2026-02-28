// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Attack;
using Code.Data.StaticData.Types.Enemies;
using Code.Gameplay.Features.Enemies.Health;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Tests.PlayMode.Helpers;

using NSubstitute;

using NUnit.Framework;

using R3;

using Tests.PlayMode.Common;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Common
{
  /// <summary>
  /// PlayMode tests for EnemyHealth.
  /// Verify: initialization with static data, TakeDamage, animation,
  /// reactive properties, edge cases.
  /// </summary>
  public class EnemyHealthTests
  {
    private GameObject _go;
    private EnemyHealth _health;
    private IAnimator _animator;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
      yield return ZenjexTestBootstrap.Initialize();

      _go = new GameObject("EnemyHealth_Test");
      _health = _go.AddComponent<EnemyHealth>();
      _animator = Substitute.For<IAnimator>();
      _health.Construct(_animator);
    }

    [TearDown]
    public void TearDown()
    {
      Object.Destroy(_go);

      ZenjexTestBootstrap.Cleanup();
    }

    #region SetCurrentValues

    [UnityTest]
    public IEnumerator SetCurrentValues_SetsMaxAndCurrentHealth_FromStaticData()
    {
      var data = CreateEnemyData(maxHealth: 200f);
      _health.SetValues(data);
      yield return null;

      Assert.That(_health.MaxHealthRP.CurrentValue, Is.EqualTo(200f));
      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(200f));
    }

    [UnityTest]
    public IEnumerator SetCurrentValues_WithZeroHealth_SetsZero()
    {
      var data = CreateEnemyData(maxHealth: 0f);
      _health.SetValues(data);
      yield return null;

      Assert.That(_health.MaxHealthRP.CurrentValue, Is.EqualTo(0f));
      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(0f));
    }

    #endregion

    #region TakeDamage

    [UnityTest]
    public IEnumerator TakeDamage_ReducesCurrentHealth()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));
      _health.TakeDamage(40f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(60f));
    }

    [UnityTest]
    public IEnumerator TakeDamage_PlaysHitAnimation()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));
      _health.TakeDamage(20f);
      yield return null;

      _animator.Received(1).PlayHit();
    }

    [UnityTest]
    public IEnumerator TakeDamage_WhenHealthIsZero_IgnoresDamage()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));
      _health.TakeDamage(100f); // killing blow
      _health.TakeDamage(50f);  // repeated damage — should be ignored
      yield return null;

      // PlayHit is invoked only once (on initial damage)
      _animator.Received(1).PlayHit();
    }

    [UnityTest]
    public IEnumerator TakeDamage_MultipleTimes_AccumulatesCorrectly()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));
      _health.TakeDamage(10f);
      _health.TakeDamage(20f);
      _health.TakeDamage(30f);
      yield return null;

      Assert.That(_health.CurrentHealthRP.CurrentValue, Is.EqualTo(40f));
    }

    #endregion

    #region ReactiveProperty

    [UnityTest]
    public IEnumerator CurrentHealthRP_EmitsNewCurrentValue_OnDamage()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));

      float received = -1f;
      _health.CurrentHealthRP
        .Skip(1)
        .Subscribe(v => received = v);

      _health.TakeDamage(35f);
      yield return null;

      Assert.That(received, Is.EqualTo(65f));
    }

    [UnityTest]
    public IEnumerator MaxHealthRP_DoesNotChange_AfterDamage()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));
      _health.TakeDamage(50f);
      yield return null;

      Assert.That(_health.MaxHealthRP.CurrentValue, Is.EqualTo(100f));
    }

    #endregion

    #region Destruction

    [UnityTest]
    public IEnumerator Destroy_DisposesReactiveProperties_WithoutException()
    {
      _health.SetValues(CreateEnemyData(maxHealth: 100f));

      Assert.DoesNotThrow(() => Object.DestroyImmediate(_go));
      yield return null;
    }

    #endregion

    #region Helper Method

    private static EnemyStaticData CreateEnemyData(float maxHealth) =>
      ScriptableObject.CreateInstance<EnemyStaticData>().Also(d =>
      {
        d.MaxHealth = maxHealth;
        d.AttackCooldown = 1f;
        d.AttackTurnSpeed = 5f;
        d.HitRecoverCooldown = 0.3f;
        d.DisappearDelay = 2f;
        d.EnemyTypeId = EnemyTypeId.Lich;
        d.EnemyAttackType = EnemyAttackType.Melee;
      });

    #endregion
  }
}
