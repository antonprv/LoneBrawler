// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.InventoryService.Interfaces;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.StateMachine.States
{
  public class BootStrapperState : IGameState
  {
    private readonly IGameLog _logger;
    private readonly GameStateMachine _gameStateMachine;
    private readonly ISceneLoader _sceneLoader;
    private readonly IAssetLoader _assetLoader;
    private readonly IInventoryService _inventoryService;
    private readonly IStaticDataService _staticData;
    private readonly IInventoryConfigSubservice _inventoryConfig;

    /// <summary>
    /// Mandatory class, initializes all other states dependencies
    /// </summary>
    /// <param name="gameStateMachine"></param>
    /// <param name="runner"></param>
    public BootStrapperState(
      GameStateMachine gameStateMachine,
      IGameLog gameLog,
      ISceneLoader sceneLoader,
      IAssetLoader assetLoader,
      IInventoryService inventoryService,
      IStaticDataService staticDataService
      )
    {
      _logger = gameLog;
      _sceneLoader = sceneLoader;
      _assetLoader = assetLoader;
      _inventoryService = inventoryService;
      _staticData = staticDataService;
      _inventoryConfig = _staticData.InventoryConfig;

      _gameStateMachine = gameStateMachine;
    }

    public async void Enter()
    {
      _logger.Log("Entered state");

      _assetLoader.Intitialize();

      await _staticData.LoadGameDataAsync();
      await _staticData.LoadInventoryConfigAsync();

      _inventoryService.Initialize(
        _inventoryConfig.InventorySize,
        _inventoryConfig.HotbarSize
        );

      _sceneLoader.Load(
        CoreScenePath.InitialSceneName,
        onSceneLoaded: EnterLoadLevel
        );
    }

    private void EnterLoadLevel()
    {
      _logger.Log($"Transitioning to {nameof(LoadProgressState)}");
      _gameStateMachine.EnterState<LoadProgressState>();
    }

    public void Exit() => _logger.Log("Exited state");
  }
}
