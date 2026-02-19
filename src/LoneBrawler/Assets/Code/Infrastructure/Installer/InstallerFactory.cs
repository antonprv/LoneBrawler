// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.UI.Elements.Utils.LoadingScreen.Interfaces;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Infrastructure.Installer
{
  public static class InstallerFactory
  {
    private static AsyncOperationHandle<GameObject> _gameInstanceHandle;
    private static AsyncOperationHandle<GameObject> _loadingScreenHandle;
    private static GameInstance _cachedGameInstance;

    public static IEnumerator CreateGameInstanceRoutine(Action<GameInstance> onComplete)
    {
      if (_cachedGameInstance != null)
      {
        onComplete?.Invoke(_cachedGameInstance);
        yield break;
      }

      _gameInstanceHandle =
          Addressables.LoadAssetAsync<GameObject>(InstallerAddresses.GameInstanceAddress);
      _loadingScreenHandle =
          Addressables.LoadAssetAsync<GameObject>(InstallerAddresses.LoadingScreenAddress);

      yield return _gameInstanceHandle;
      yield return _loadingScreenHandle;

      if (_gameInstanceHandle.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"{nameof(InstallerFactory)}: Failed to load GameInstance");
        onComplete?.Invoke(null);
        yield break;
      }

      if (_loadingScreenHandle.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"{nameof(InstallerFactory)}: Failed to load LoadingScreen");
        onComplete?.Invoke(null);
        yield break;
      }

      _cachedGameInstance = InstantiateGameInstance(
          _gameInstanceHandle.Result,
          _loadingScreenHandle.Result);

      onComplete?.Invoke(_cachedGameInstance);
    }

    public static void Release()
    {
      _cachedGameInstance = null;

      if (_gameInstanceHandle.IsValid())
        Addressables.Release(_gameInstanceHandle);

      if (_loadingScreenHandle.IsValid())
        Addressables.Release(_loadingScreenHandle);
    }

    private static GameInstance InstantiateGameInstance(
        GameObject gameInstancePrefab,
        GameObject loadingScreenPrefab)
    {
      GameObject gameInstanceObject = CreateGameInstance(gameInstancePrefab);
      GameObject loadingScreenObject = CreateLoadingScreen(loadingScreenPrefab);
      return ConfigureComponents(gameInstanceObject, loadingScreenObject);
    }

    private static GameInstance ConfigureComponents(GameObject gameInstanceObject, GameObject loadingScreenObject)
    {
      ILoadScreen loadScreen =
        loadingScreenObject.GetComponent<ILoadScreen>();

      GameInstance gameInstance =
        gameInstanceObject.GetComponent<GameInstance>();

      gameInstance.Construct(loadScreen);

      return gameInstance;
    }

    private static GameObject CreateLoadingScreen(GameObject loadingScreenPrefab)
    {
      GameObject loadingScreenObject =
        UnityEngine.Object.Instantiate(loadingScreenPrefab);

      UnityEngine.Object.DontDestroyOnLoad(loadingScreenObject);
      return loadingScreenObject;
    }

    private static GameObject CreateGameInstance(GameObject gameInstancePrefab)
    {
      GameObject gameInstanceObject =
        UnityEngine.Object.Instantiate(gameInstancePrefab);

      UnityEngine.Object.DontDestroyOnLoad(gameInstanceObject);
      return gameInstanceObject;
    }
  }
}
