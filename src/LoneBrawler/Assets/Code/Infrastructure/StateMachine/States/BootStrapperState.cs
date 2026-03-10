// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Services.InventoryService.Interfaces;

using Cysharp.Threading.Tasks;

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
    /// Preloads configs and static data.
    /// </summary>
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
      await _staticData.LoadMusicConfigAsync();

      _inventoryService.Initialize(
        _inventoryConfig.InventorySize,
        _inventoryConfig.HotbarSize
        );

      await _sceneLoader.LoadPlatformBased(
        CoreScenePath.InitialSceneName,
        _staticData.BuildConfig.TargetPlatform,
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
