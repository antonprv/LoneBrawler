// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

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
    private static GameObject _gameInstancePrefab;
    private static GameObject _loadingScreenPrefab;

    #region Public API

    public static GameInstance CreateGameInstance()
    {
      if (_cachedGameInstance != null)
        return _cachedGameInstance;

      if (!IsWarmUpSuccessfull()) return null;

      _cachedGameInstance = InstantiateGameInstance();

      return _cachedGameInstance;
    }

    public static void Release()
    {
      _cachedGameInstance = null;

      if (_gameInstanceHandle.IsValid())
        Addressables.Release(_gameInstanceHandle);

      if (_loadingScreenHandle.IsValid())
        Addressables.Release(_loadingScreenHandle);
    }

    #endregion

    #region Private API

    private static bool IsWarmUpSuccessfull()
    {
      _gameInstancePrefab =
        LoadAddressable(ref _gameInstanceHandle, InstallerAddresses.GameInstanceAddress);
      if (_gameInstancePrefab == null) return false;

      _loadingScreenPrefab =
        LoadAddressable(ref _loadingScreenHandle, InstallerAddresses.LoadingScreenAddress);
      if (_loadingScreenPrefab == null) return false;

      return true;
    }

    private static GameObject LoadAddressable(
        ref AsyncOperationHandle<GameObject> handle,
        string address)
    {
      if (!handle.IsValid())
        handle = Addressables.LoadAssetAsync<GameObject>(address);

      GameObject prefab = handle.WaitForCompletion();

      if (prefab == null)
        Debug.LogError($"{nameof(InstallerFactory)}:" +
          $" Couldn't load addressable at address: {address}");

      return prefab;
    }

    private static GameInstance InstantiateGameInstance()
    {
      GameObject gameInstanceObject = CreateGameInstance(_gameInstancePrefab);
      GameObject loadingScreenObject = CreateLoadingScreen(_loadingScreenPrefab);

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
        Object.Instantiate(loadingScreenPrefab);

      Object.DontDestroyOnLoad(loadingScreenObject);
      return loadingScreenObject;
    }

    private static GameObject CreateGameInstance(GameObject gameInstancePrefab)
    {
      GameObject gameInstanceObject =
        Object.Instantiate(gameInstancePrefab);

      Object.DontDestroyOnLoad(gameInstanceObject);
      return gameInstanceObject;
    }

    #endregion
  }
}
