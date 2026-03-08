// Created by Anton Piruev in 2026.
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.UI.Elements.Common.LoadingScreen.Interfaces;

using UnityEngine;
using UObject = UnityEngine.Object;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Infrastructure.Installer
{
  /// <summary>
  /// Responsible for async Addressable loading and instantiation of
  /// infrastructure prefabs that must exist before the game loop starts.
  ///
  /// Two separate routines keep concerns isolated:
  ///   1. <see cref="CreateLoadingScreenRoutine"/> — instantiates the curtain and
  ///      returns <see cref="ILoadScreen"/>. Must be called first so the caller can
  ///      register ILoadScreen in the DI container before GameInstance is created.
  ///   2. <see cref="CreateGameInstanceRoutine"/> — instantiates GameInstance.
  ///      At this point ILoadScreen is already in the container, so
  ///      ZenjexBehaviour.Awake() resolves the [Zenjex] field automatically.
  /// </summary>
  public static class InstallerFactory
  {
    private static AsyncOperationHandle<GameObject> _loadingScreenHandle;
    private static AsyncOperationHandle<GameObject> _gameInstanceHandle;

    #region Loading Screen

    /// <summary>
    /// Loads and instantiates the LoadingScreen prefab.
    /// Call this before <see cref="CreateGameInstanceRoutine"/> and register
    /// the result as ILoadScreen in the DI container immediately after.
    /// </summary>
    public static IEnumerator CreateLoadingScreenRoutine(Action<ILoadScreen> onComplete)
    {
      _loadingScreenHandle =
          Addressables.LoadAssetAsync<GameObject>(InstallerAddresses.LoadingScreenAddress);

      yield return _loadingScreenHandle;

      if (_loadingScreenHandle.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"{nameof(InstallerFactory)}: Failed to load LoadingScreen prefab.");
        onComplete?.Invoke(null);
        yield break;
      }

      GameObject go = UObject.Instantiate(_loadingScreenHandle.Result);
      UObject.DontDestroyOnLoad(go);

      onComplete?.Invoke(go.GetComponent<ILoadScreen>());
    }

    #endregion

    #region Game Instance

    /// <summary>
    /// Loads and instantiates the GameInstance prefab.
    ///
    /// ILoadScreen MUST be registered in RootContainer before calling this.
    /// GameInstance inherits ZenjexBehaviour, so Unity's Instantiate() triggers
    /// ZenjexBehaviour.Awake() → ZenjexInjector.Inject(this), which resolves
    /// the [Zenjex] ILoadScreen field from the container automatically.
    /// No manual Construct() call is required.
    /// </summary>
    public static IEnumerator CreateGameInstanceRoutine(Action<GameInstance> onComplete)
    {
      _gameInstanceHandle =
          Addressables.LoadAssetAsync<GameObject>(InstallerAddresses.GameInstanceAddress);

      yield return _gameInstanceHandle;

      if (_gameInstanceHandle.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"{nameof(InstallerFactory)}: Failed to load GameInstance prefab.");
        onComplete?.Invoke(null);
        yield break;
      }

      // ZenjexBehaviour.Awake() fires here and injects [Zenjex] ILoadScreen
      GameObject go = UObject.Instantiate(_gameInstanceHandle.Result);
      UObject.DontDestroyOnLoad(go);

      onComplete?.Invoke(go.GetComponent<GameInstance>());
    }

    #endregion

    #region Cleanup

    public static void Release()
    {
      if (_loadingScreenHandle.IsValid())
        Addressables.Release(_loadingScreenHandle);

      if (_gameInstanceHandle.IsValid())
        Addressables.Release(_gameInstanceHandle);
    }

    #endregion
  }
}
