// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.CustomTypes.Types;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Gameplay.Features.Player.Health;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Player;
using Code.UI.Elements.Utils.LoadingScreen.Interfaces;
using Code.UI.Factory.Interfaces;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadLevelState : IGamePayloadedState<string>
  {
    private readonly IGameLog _logger;

    private readonly GameStateMachine _gameStateMachine;
    private readonly ICoroutineRunner _runner;
    private readonly ILoadScreen _curtain;

    private ISceneLoader _sceneLoader;
    private IGameFactory _gameFactory;
    private ICameraManager _cameraManager;
    private string _loadedSceneName;
    private LevelStaticData _levelData;
    private bool _wasHudCreated;
    private GameObject _hud;
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IStaticDataService _staticDataService;
    private readonly IUIFactory _uiFactory;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IAssetProvider _assetProvider;
    private readonly IPlayerWriter _playerWriter;
    private readonly IPlayerReader _playerReader;

    public LoadLevelState(
      GameStateMachine gameStateMachine,
      ICoroutineRunner runner,
      ILoadScreen curtain)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _sceneLoader = RootContext.Resolve<ISceneLoader>();
      _gameFactory = RootContext.Resolve<IGameFactory>();
      _cameraManager = RootContext.Resolve<ICameraManager>();
      _persistentProgressService = RootContext.Resolve<IPersistentProgressService>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();
      _uiFactory = RootContext.Resolve<IUIFactory>();
      _saveLoadService = RootContext.Resolve<ISaveLoadService>();
      _assetProvider = RootContext.Resolve<IAssetProvider>();

      _playerWriter = RootContext.Resolve<IPlayerWriter>();
      _playerReader = RootContext.Resolve<IPlayerReader>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;

      _curtain = curtain;
    }

    public async void Enter(string payload)
    {
      _logger.Log("Entered state");

      _curtain.Show();
      _gameFactory.Cleanup();
      _uiFactory.Cleanup();
      await _gameFactory.WarmUp();
      await _uiFactory.WarmUp();

      await _sceneLoader.LoadAsync(payload, onSceneLoaded: OnLevelLoadedAsync);
    }
    public void Exit()
    {
      _logger.Log("Exited state");

      _curtain.Hide();
    }

    private async void OnLevelLoadedAsync()
    {
      _logger.Log("Loading content for the active level...");

      LoadLevelData();

      InitUIRoot();
      await InitGameWorldAsync();
      InformProgressReadersAsync();

      MakeFirstSave();

      _gameStateMachine.EnterState<GameLoopState>();
    }

    private void MakeFirstSave() => _saveLoadService.SaveProgress();

    private void LoadLevelData()
    {
      _loadedSceneName = SceneManager.GetActiveScene().name;
      _levelData = _staticDataService.LevelData.ForLevel(_loadedSceneName);
    }

    private void InitUIRoot() => _uiFactory.CreateUIRootAsync();

    private void InformProgressReadersAsync()
    {
      foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
        progressReader.ReadProgress(_persistentProgressService.Progress);
    }

    private async Task InitGameWorldAsync()
    {
      GameObject player = await InitPlayerAsync();
      await InitSpawnersAsync();
      await InitHudAsync(player);
      await InitLevelTeleports();
    }

    private async Task<GameObject> InitPlayerAsync()
    {
      CleanupPlayer();

      GameObject player = await _gameFactory
        .CreateAndPlacePlayerAsync(GetPlayerCoordinates());

      _cameraManager.Follow(player);
      _playerWriter.SetPlayer(player);

      return player;
    }

    private void CleanupPlayer()
    {
      if (_playerReader.GetPlayer() != null)
        Object.Destroy(_playerReader.GetPlayer());
    }

    private Coordinates GetPlayerCoordinates()
    {
      Coordinates playerSpawnCoords = _levelData.Teleports
        .Find(
          x => x.UniqueName == _persistentProgressService
          ?.Progress
          ?.PlayerWorldData
          ?.LastTeleportUniqueName
        )
        ?.PlayerSpawnCoords;

      return playerSpawnCoords ?? _levelData.PlayerStartCoordinates;
    }

    private async Task InitSpawnersAsync()
    {
      foreach (var spawnerData in _levelData.EnemySpawners)
      {
        _gameFactory.CreateEnemySpawner(
            spawnerData.Position,
            spawnerData.SpawnerId,
            spawnerData.EnemyTypeId);

        await Task.Yield();
      }
    }

    private async Task InitHudAsync(GameObject player)
    {
      CleanupHud();
      _hud = await _gameFactory.CreateHudAsync();
      _hud.GetComponent<PlayerUI>()
        .Construct(player.GetComponent<PlayerHealth>());
    }

    private void CleanupHud()
    {
      if (_hud != null)
        GameObject.Destroy(_hud);
    }

    private async Task InitLevelTeleports()
    {
      foreach (var levelTeleport in _levelData.Teleports)
      {
        _gameFactory.CreateTeleport(
          coords: levelTeleport.Coords,
          scale: levelTeleport.Scale,
          levelKey: levelTeleport.LevelKey,
          uniqueName: levelTeleport.UniqueName
          );

        await Task.Yield();
      }
    }
  }
}
