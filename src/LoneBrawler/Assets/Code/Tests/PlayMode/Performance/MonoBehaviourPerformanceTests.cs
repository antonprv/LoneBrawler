// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Features.Enemies.Health;
using Code.Gameplay.Features.Player.Health;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Tests.PlayMode.Helpers;

using NSubstitute;

using Tests.PlayMode.Common;

using Unity.PerformanceTesting;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Performance
{
  public class MonoBehaviourPerformanceTests
  {
    #region PlayerHealth.TakeDamage

    [UnityTest, Performance]
    public IEnumerator PlayerHealth_TakeDamage_PerformanceUnder1ms()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // CreatePlayerGameObject adds CharacterController + PlayerAnimator + PlayerMove
      // so PlayerAnimator.Update() won't throw NullReferenceException during measurement frames
      var go = ZenjexTestBootstrap.CreatePlayerGameObject("Perf_PlayerHealth");
      var health = go.AddComponent<PlayerHealth>();
      var animator = Substitute.For<IAnimator>();
      health.Construct(animator);

      var progress = TestHelpers.CreateProgress(maxHealth: 1_000_000f, currentHealth: 1_000_000f);
      health.ReadProgress(progress);

      Measure.Method(() =>
      {
        for (int i = 0; i < 100; i++)
          health.TakeDamage(0.001f);
      })
      .WarmupCount(5)
      .MeasurementCount(20)
      .IterationsPerMeasurement(1)
      .Run();

      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    #endregion

    #region EnemyHealth.TakeDamage

    [UnityTest, Performance]
    public IEnumerator EnemyHealth_TakeDamage_PerformanceUnder1ms()
    {
      yield return ZenjexTestBootstrap.Initialize();

      var go = new GameObject("Perf_EnemyHealth");
      var health = go.AddComponent<EnemyHealth>();
      var animator = Substitute.For<IAnimator>();
      health.Construct(animator);

      var data = ScriptableObject.CreateInstance<Code.Data.StaticData.EnemyStaticData>();
      data.MaxHealth = 1_000_000f;
      health.SetValues(data);

      Measure.Method(() =>
      {
        for (int i = 0; i < 100; i++)
          health.TakeDamage(0.001f);
      })
      .WarmupCount(5)
      .MeasurementCount(20)
      .IterationsPerMeasurement(1)
      .Run();

      yield return null;
      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    #endregion

    #region Creating and Destruction of Objects

    [UnityTest, Performance]
    public IEnumerator GameObject_CreateAndDestroy_WithComponents_Under5ms()
    {
      yield return ZenjexTestBootstrap.Initialize();

      Measure.Method(() =>
      {
        for (int i = 0; i < 10; i++)
        {
          // Use CreatePlayerGameObject so the PlayerHealth's [RequireComponent]
          // dependencies are present and no NullReferenceException occurs on Awake/Update
          var go = ZenjexTestBootstrap.CreatePlayerGameObject($"PerfGO_{i}");
          go.AddComponent<PlayerHealth>();
          Object.DestroyImmediate(go);
        }
      })
      .WarmupCount(3)
      .MeasurementCount(10)
      .IterationsPerMeasurement(1)
      .Run();

      yield return null;

      ZenjexTestBootstrap.Cleanup();
    }

    #endregion

    #region Player Health Frame Time

    [UnityTest, Performance]
    public IEnumerator FrameTime_WithActivePlayerHealth_IsStable()
    {
      yield return ZenjexTestBootstrap.Initialize();

      // CreatePlayerGameObject ensures PlayerAnimator and PlayerMove are present,
      // so the game loop ticks cleanly across all measurement frames
      var go = ZenjexTestBootstrap.CreatePlayerGameObject("Perf_FrameTime");
      var health = go.AddComponent<PlayerHealth>();
      var animator = Substitute.For<IAnimator>();
      health.Construct(animator);

      var progress = TestHelpers.CreateProgress(maxHealth: 100f, currentHealth: 100f);
      health.ReadProgress(progress);

      yield return Measure.Frames()
        .WarmupCount(5)
        .MeasurementCount(20)
        .Run();

      Object.Destroy(go);

      ZenjexTestBootstrap.Cleanup();
    }

    #endregion
  }
}
