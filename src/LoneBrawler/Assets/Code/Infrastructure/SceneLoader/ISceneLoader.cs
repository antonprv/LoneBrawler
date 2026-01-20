// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.Extensions.Async;

namespace Code.Infrastructure.SceneLoader
{
  public interface ISceneLoader
  {
    public void Load(string name, ICoroutineRunner runner, Action onSceneLoaded = null, float waitSeconds = 0.01f);
  }
}
