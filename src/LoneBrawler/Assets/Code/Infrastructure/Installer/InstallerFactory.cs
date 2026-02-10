// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Utils.LoadingScreen.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Installer
{
  public static class InstallerFactory
  {
    public static GameInstance CreateGameInstance()
    {
      var gameInstancePrefab =
        Resources.Load<GameObject>(InstallerPaths.GameInstancePath);
      GameObject gameInstanceObject = Object.Instantiate(gameInstancePrefab);

      var loadingScreenPrefab = Resources.Load<GameObject>(InstallerPaths.LoadingScreenPath);
      GameObject loadingScreenObject = Object.Instantiate(loadingScreenPrefab);
      ILoadScreen loadScreen = loadingScreenObject.GetComponent<ILoadScreen>();

      var gameInstance = gameInstanceObject.GetComponent<GameInstance>();
      gameInstance.Construct(loadScreen);

      return gameInstance;
    }
  }
}
