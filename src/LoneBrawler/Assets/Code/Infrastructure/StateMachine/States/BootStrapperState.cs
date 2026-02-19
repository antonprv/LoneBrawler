// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  public class BootStrapperState : IGameState
  {
    private readonly IGameLog _logger;
    private readonly IStaticDataService _staticData;
    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private ISceneLoader _sceneLoader;
    private readonly IAssetLoader _assetLoader;

    /// <summary>
    /// Mandatory class, initializes all other states dependencies
    /// </summary>
    /// <param name="gameStateMachine"></param>
    /// <param name="runner"></param>
    public BootStrapperState(
      GameStateMachine gameStateMachine)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _sceneLoader = RootContext.Resolve<ISceneLoader>();
      _assetLoader = RootContext.Resolve<IAssetLoader>();

      _gameStateMachine = gameStateMachine;
    }

    public void Enter()
    {
      _logger.Log("Entered state");

      _assetLoader.Intitialize();

      _sceneLoader.Load(
        CoreScenePath.InitialSceneName,
        onSceneLoaded: EnterLoadLevel
        );
    }

    private void EnterLoadLevel()
    {
      _logger.Log($"Transitioning to {nameof(LoadProgress)}");
      _gameStateMachine.EnterState<LoadProgress>();
    }

    public void Exit() => _logger.Log("Exited state");
  }
}
