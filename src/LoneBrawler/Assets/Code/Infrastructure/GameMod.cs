// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Gameplay.Common.Visuals.UI;
using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.StateMachine;

namespace Code.Infrastructure
{
  public struct GameStateDependencies
  {
    public ISceneLoader sceneLoader;
    public IGameFactory gameFactory;
    public ICameraManager cameraManager;
  }

  public class GameMod
  {
    public GameStateMachine StateMachine { get; private set; }

    public GameStateDependencies Dependencies { get; private set; }

    public GameMod(ICoroutineRunner runner, ILoadScreen loadScreen, GameStateDependencies stateData)
    {
      StateMachine =
        new GameStateMachine(
          runner,
          stateData.sceneLoader,
          stateData.gameFactory,
          stateData.cameraManager,
          loadScreen);
    }
  }
}
