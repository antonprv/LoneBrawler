// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;
using System.Threading.Tasks;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.SceneLoader.Interfaces;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.SceneLoader
{
  public class SceneLoader : ISceneLoader
  {
    private readonly IGameLog _logger;

    public SceneLoader()
    {
      _logger = RootContext.Resolve<IGameLog>();
    }

    public void Load(
      string name, ICoroutineRunner runner, Action onSceneLoaded = null, float waitSeconds = 0.01f) =>
      runner.StartCoroutine(LoadScene(name, onSceneLoaded, waitSeconds));

    public async Task LoadAsync(string address, Action onSceneLoaded = null, int waitMilieconds = 10)
    {
      if (SceneManager.GetActiveScene().name == address)
      {
        _logger.Log($"{address} was already loaded. Skipping...");
        onSceneLoaded?.Invoke();
        return;
      }

      AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address);
      await handle.Task;
      await Task.Delay(waitMilieconds);
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
