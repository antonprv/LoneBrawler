// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;
using System.Threading.Tasks;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Configs.Types;
using Code.Infrastructure.SceneLoader.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.SceneLoader
{
  public class AddressableSceneLoader : ISceneLoader
  {
    private readonly IGameLog _logger;
    private readonly ICoroutineRunner _runner;

    public AddressableSceneLoader(
      IGameLog gameLog,
      ICoroutineRunner coroutineRunner
      )
    {
      _logger = gameLog;
      _runner = coroutineRunner;
    }

    public void Load(string name, Action onSceneLoaded = null, float waitSeconds = 0.01f) =>
      _runner.StartCoroutine(LoadScene(name, onSceneLoaded, waitSeconds));

    public async UniTask LoadPlatformBased(
      string nameOrAddress,
      TargetPlatform platform,
      Action onSceneLoaded = null,
      float waitSeconds = 0.01f
      )
    {
      switch (platform)
      {
        case TargetPlatform.None:
          break;
        case TargetPlatform.WebGL:
          LoadAddressable(nameOrAddress, onSceneLoaded, waitSeconds);
          break;
        case TargetPlatform.Android:
          await LoadAsync(nameOrAddress, onSceneLoaded, (int)waitSeconds);
          break;
        default:
          break;
      }
    }

    public void LoadAddressable(
      string address,
      Action onSceneLoaded = null,
      float WaitSeconds = 0.01f) =>
      _runner.StartCoroutine(LoadAsyncWithCoroutine(address, onSceneLoaded, WaitSeconds));

    private IEnumerator LoadAsyncWithCoroutine(string address, Action onSceneLoaded, float waitSeconds)
    {
      yield return null;

      if (SceneManager.GetActiveScene().name == address)
      {
        _logger.Log($"{address} was already loaded. Skipping...");
        onSceneLoaded?.Invoke();
        yield break;
      }

      var sceneLoadhandle = Addressables.LoadSceneAsync(address);

      while (!sceneLoadhandle.IsDone)
        yield return new WaitForSeconds(waitSeconds);

      if (sceneLoadhandle.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"{nameof(AddressableSceneLoader)}: Failed to load scene {address}");
        yield break;
      }

      yield return null;

      onSceneLoaded?.Invoke();
    }

    public async UniTask LoadAsync(string address, Action onSceneLoaded = null, int waitMilieconds = 10)
    {
      if (SceneManager.GetActiveScene().name == address)
      {
        _logger.Log($"{address} was already loaded. Skipping...");
        onSceneLoaded?.Invoke();
        return;
      }

      AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address);
      await handle.Task;

      if (handle.Status != AsyncOperationStatus.Succeeded)
        throw new Exception($"Failed to load scene: {address}. Status: {handle.Status}");

      await UniTask.Delay(waitMilieconds);
      onSceneLoaded?.Invoke();
    }

    private IEnumerator LoadScene(string sceneName, Action onSceneLoaded = null, float waitSeconds = 0.01f)
    {
      if (SceneManager.GetActiveScene().name == sceneName)
      {
        _logger.Log($"{sceneName} was already loaded. Skipping...");
        onSceneLoaded?.Invoke();
        yield break;
      }

      AsyncOperation _loadOperation = SceneManager.LoadSceneAsync(sceneName);

      while (!_loadOperation.isDone)
        yield return new WaitForSeconds(waitSeconds);

      onSceneLoaded?.Invoke();
      _logger.Log($"{sceneName} was loaded successfully.");
    }

  }
}
