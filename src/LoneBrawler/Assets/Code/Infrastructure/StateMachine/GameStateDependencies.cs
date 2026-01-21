// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.PersistentProgress;

namespace Code.Infrastructure
{
  public sealed class GameStateDependencies
  {
    public ISceneLoader sceneLoader;
    public IGameFactory gameFactory;
    public ICameraManager cameraManager;
    public IPersistentProgressService progressService;

    /// <summary>
    /// Dependency injection container, specifically for StateMachine dependencies
    /// </summary>
    /// <param name="sceneLoader"></param>
    /// <param name="gameFactory"></param>
    /// <param name="cameraManager"></param>
    /// <param name="progressService"></param>
    public GameStateDependencies(
      ISceneLoader sceneLoader,
      IGameFactory gameFactory,
      ICameraManager cameraManager,
      IPersistentProgressService progressService
      )
    {
      this.sceneLoader = sceneLoader;
      this.gameFactory = gameFactory;
      this.cameraManager = cameraManager;
      this.progressService = progressService;
    }
  }
}
