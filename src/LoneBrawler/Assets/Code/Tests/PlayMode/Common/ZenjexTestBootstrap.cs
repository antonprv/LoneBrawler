// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;
using System.Reflection;

using Code.Common.Extensions.Logging;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Features.Player.Movement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using NSubstitute;

using Reflex.Core;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Tests.PlayMode.Common
{
  public static class ZenjexTestBootstrap
  {
    private static GameObject _installerGO;

    /// <summary>
    /// Creates and initializes minimal DI container for PlayMode tests.
    /// Call in [UnitySetUp] before creating any ZenjexBehaviour components.
    /// yield return is mandatory - gives Unity one frame to invoke Awake().
    /// </summary>
    public static IEnumerator Initialize()
    {
      // If container already exists - do nothing
      if (RootContext.HasInstance)
        yield break;

      _installerGO = new GameObject("[TestRootInstaller]");
      _installerGO.AddComponent<TestRootInstaller>();

      // Wait one frame - Unity will call Awake() on TestRootInstaller,
      // which builds the container and populates ProjectRootInstaller.RootContainer
      yield return null;
    }

    /// <summary>
    /// Destroys the container and installer object.
    /// Call in [TearDown] or [UnityTearDown].
    /// Mandatory - ProjectRootInstaller.RootContainer is static,
    /// without cleanup it will leak into next test.
    /// </summary>
    public static void Cleanup()
    {
      // Reset static RootContainer field via reflection
      // (it has private set, direct access is impossible)
      var prop = typeof(ProjectRootInstaller).GetProperty(
        "RootContainer",
        BindingFlags.Public | BindingFlags.Static);
      prop?.SetValue(null, null);

      if (_installerGO != null)
      {
        Object.Destroy(_installerGO);
        _installerGO = null;
      }
    }

    /// <summary>
    /// Creates a GameObject with PlayerMove and PlayerAnimator properly wired up.
    ///
    /// Use this instead of bare new GameObject() whenever the test involves
    /// PlayerDeath (which has [RequireComponent(typeof(PlayerAnimator))] and
    /// [RequireComponent(typeof(PlayerMove))]).
    ///
    /// Without this, Unity auto-adds both components the moment AddComponent<PlayerDeath>()
    /// is called, and they start ticking immediately - before Zenjex has a chance to
    /// inject ITimeService, IInputService, etc. - which causes cascading NullReferenceExceptions.
    ///
    /// What this helper does:
    ///   1. Adds CharacterController (required by both PlayerAnimator and PlayerMove).
    ///   2. Adds Animator stub (required by PlayerAnimator).
    ///   3. Adds PlayerAnimator and assigns its public fields so Update() has no nulls.
    ///   4. Adds PlayerMove and assigns CharacterController so IsMovementForbidden() works.
    ///   5. Disables both components so their Update() loops do NOT run during tests,
    ///      since the tests don't exercise movement or animation logic directly.
    /// </summary>
    public static GameObject CreatePlayerGameObject(string name = "Player_Test")
    {
      var go = new GameObject(name);

      // CharacterController is a required public field on both PlayerAnimator and PlayerMove
      var cc = go.AddComponent<CharacterController>();

      // PlayerAnimator.Update() reads Animator and CharacterController
      // Animator is a Unity built-in component - AddComponent gives a usable stub
      var unityAnimator = go.AddComponent<Animator>();

      var playerAnimator = go.AddComponent<PlayerAnimator>();
      playerAnimator.Animator = unityAnimator;
      playerAnimator.CharacterController = cc;
      // Disable so Update() doesn't fire during tests that don't need animation ticking
      playerAnimator.enabled = false;

      var playerMove = go.AddComponent<PlayerMove>();
      playerMove.CharacterController = cc;
      // Disable so Update() → MovePlayer() doesn't fire, and OnDestroy()
      // won't crash because _attacker was never set via Construct()
      playerMove.enabled = false;

      return go;
    }

    #region Register installer for tests

    /// <summary>
    /// Minimal implementation of ProjectRootInstaller.
    /// Registers NSubstitute-mocks for all [Zenjex]-dependencies
    /// used by components in PlayMode tests:
    ///   - PlayerAnimator  → ITimeService
    ///   - PlayerMove      → ITimeService, IInputService, IPlayerDataSubervice
    ///   - EnemyAttack     → ITimeService
    ///   - (others)        → IGameConfigSubservice, IGameLog
    ///
    /// If test adds component with unregistered dependency -
    /// ZenjexInjector logs error but won't crash.
    /// Add necessary types to InstallBindings as needed.
    /// </summary>
    private class TestRootInstaller : ProjectRootInstaller
    {
      public override void InstallBindings(ContainerBuilder builder)
      {
        // Register mocks as values (essentially singletons)
        builder.RegisterValue(Substitute.For<ITimeService>(), new[] { typeof(ITimeService) });
        builder.RegisterValue(Substitute.For<IInputService>(), new[] { typeof(IInputService) });
        builder.RegisterValue(Substitute.For<IPlayerDataSubervice>(), new[] { typeof(IPlayerDataSubervice) });
        builder.RegisterValue(Substitute.For<IGameConfigSubservice>(), new[] { typeof(IGameConfigSubservice) });
        builder.RegisterValue(Substitute.For<IGameLog>(), new[] { typeof(IGameLog) });
        builder.RegisterValue(Substitute.For<IAssetLoader>(), new[] { typeof(IAssetLoader) });
      }

      public override IEnumerator InstallGameInstanceRoutine()
      {
        yield break; // Tests don't require game initialization
      }

      public override void LaunchGame()
      {
        // nothing gets launched
      }

    #endregion
    }
  }
}
