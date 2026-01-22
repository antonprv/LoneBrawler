// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

using UnityEngine;
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
      string name, ICoroutineRunner runner, Action onSceneLoaded = null, float waitSeconds = 0.01F) =>
      runner.StartCoroutine(LoadScene(name, onSceneLoaded, waitSeconds));

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
