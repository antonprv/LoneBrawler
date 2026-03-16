// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.LocalisationService;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.Infrastructure.StateMachine.Types;
using Code.UI.Services.InventoryService.Interfaces;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StateMachine.States
{
  public class BootStrapperState : IGameState
  {
    #region StateType

    public StateType Type => StateType.BootStrapper;

    #endregion

    private readonly IGameLog _logger;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly ILocalisationService _localisation;
    private readonly ISceneLoader _sceneLoader;
    private readonly IAssetLoader _assetLoader;
    private readonly IInventoryService _inventoryService;
    private readonly IStaticDataService _staticData;
    private readonly IInventoryConfigSubservice _inventoryConfig;
    private readonly IInputService _inputService;
    private readonly IDevConsole _devConsole;
    private readonly IConsoleComponent _consoleComponent;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Preloads configs and static data.
    /// </summary>
    public BootStrapperState(
      IGameStateMachine gameStateMachine,
      IGameLog gameLog,
      ISceneLoader sceneLoader,
      IAssetLoader assetLoader,
      IInventoryService inventoryService,
      IStaticDataService staticDataService,
      IInputService inputService,
      IDevConsole devConsole,
      IConsoleComponent consoleComponent,
      ILocalisationService localisationService
      )
    {
      _logger = gameLog;
      _sceneLoader = sceneLoader;
      _assetLoader = assetLoader;
      _inventoryService = inventoryService;
      _staticData = staticDataService;
      _inventoryConfig = _staticData.InventoryConfig;
      _inputService = inputService;

      _devConsole = devConsole;
      _consoleComponent = consoleComponent;

      _gameStateMachine = gameStateMachine;

      _localisation = localisationService;
    }

    public void Enter() => EnterAsync().Forget();

    private async UniTask EnterAsync()
    {
      _logger.Log("Entered state");

      _cts = new CancellationTokenSource();
      var ct = _cts.Token;

      _localisation.Initialize();
      _inputService.GameInputEnabled = false;

      await _assetLoader.Intitialize();

      await _staticData.LoadBuildDataAsync();
      await _staticData.LoadGameDataAsync();
      await _staticData.LoadInventoryConfigAsync();
      await _staticData.LoadMusicConfigAsync();

      InitializeDevConcole();

      if (ct.IsCancellationRequested) return;

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

    public void Exit()
    {
      _logger.Log("Exited state");
      _cts?.Cancel();
      _cts?.Dispose();
      _cts = null;
    }

    private void InitializeDevConcole()
    {
      if (_devConsole != null)
      {
        _devConsole.Initialize();
        _consoleComponent.InitializeCommands();
      }
    }

    private void EnterLoadLevel()
    {
      _logger.Log($"Transitioning to {nameof(LoadProgressState)}");
      _gameStateMachine.EnterState<LoadProgressState>();
    }


  }
}
