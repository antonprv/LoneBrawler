// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Threading.Tasks;

using Code.Data.StaticData.Configs.Types;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.SceneLoader.Interfaces
{
  public interface ISceneLoader
  {
    public void Load(string name, Action onSceneLoaded = null, float waitSeconds = 0.01f);
    void LoadAddressable(string address, Action onSceneLoaded = null, float WaitSeconds = 0.01F);
    UniTask LoadAsync(string address, Action onSceneLoaded = null, int waitMilieconds = 10);
    UniTask LoadPlatformBased(
      string nameOrAddress,
      TargetPlatform platform,
      Action onSceneLoaded = null,
      float waitSeconds = 0.01F
      );
  }
}
