// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.Async;
using Code.Infrastructure;

namespace Assets.Code.Gameplay.Services.SceneLoader
{
  public interface ISceneLoader
  {
    void Load(string name, ICoroutineRunner runner, Action onSceneLoaded = null, float waitSeconds = 0.01f);
  }
}
