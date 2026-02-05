// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Data.StaticData.DataReceivers;
using Code.Data.StaticData.Types;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Features.Enemies.Aggro.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Gameplay.Features.Enemies.Spawn;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Gameplay.Features.Player.Metadata.Interfaces;
using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Gameplay.Services.Random;
using Code.Gameplay.Services.Time;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.UI.Elements;
using Code.UI.Services.WindowService.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Factory
{
  public class GameFactory : IGameFactory
  {
    private readonly IGameLog _logger;
    private readonly IAssetProvider _assetProvider;
    private readonly IStaticDataService _staticDataService;
    private readonly IPlayerReader _playerReader;
    private readonly IRandomService _randomService;
    private readonly IInputService _inputService;
    private readonly ITimeService _timeService;
    private readonly IWindowService _windowService;
    private readonly IEnemyDataSubservice _enemyDataService;
    private readonly IBuildConfigSubservice _buildConfig;
    private readonly IGameConfigSubservice _gameConfig;
    private string _playerStartTag;

    public GameFactory()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _assetProvider = RootContext.Resolve<IAssetProvider>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _randomService = RootContext.Resolve<IRandomService>();
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();
      _windowService = RootContext.Resolve<IWindowService>();

      _enemyDataService = _staticDataService.EnemyData;
      _buildConfig = _staticDataService.BuildConfig;
      _gameConfig = _staticDataService.GameConfig;
      _playerStartTag = _gameConfig.PlayerStartTag;
    }

    public List<IProgressReader> ProgressReaders { get; } = new List<IProgressReader>();
    public List<IProgressWriter> ProgressWriters { get; } = new List<IProgressWriter>();

    /*-----------------public API-----------------------*/

    public GameObject CreatePlayer() =>
      InitializePlayerComponents(
        InstantiateRegistered(AssetPaths.PlayerPath)
        );

    public GameObject CreateAndPlacePlayer() =>
      InitializePlayerComponents(
        PlacePlayer(player: InstantiateRegistered(AssetPaths.PlayerPath))
        );

    public GameObject CreateHud() => InstantiateHud();

    public void CreateEnemySpawner(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId) =>
      InstantiateSpawner(at, spawnerId, enemyTypeId);

    public GameObject CreateEnemy(EnemyTypeId typeId, Transform parent) =>
      InstantiateEnemy(typeId, parent);

    public GameObject CreateLoot(EnemyTypeId typeId, Vector3 position) =>
      InstantiateLoot(typeId, position);

    public void Cleanup()
    {
      ProgressReaders.Clear();
      ProgressWriters.Clear();
    }

    /*-----------------private methods------------------*/

    private GameObject InstantiateHud()
    {
      GameObject hudObject = InstantiateRegistered(AssetPaths.HudPath);

      foreach (OpenWindowButton button
        in hudObject.GetComponentsInChildren<OpenWindowButton>())
        button.Construct(_windowService);

      return hudObject;
    }

    private void InstantiateSpawner(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId)
    {
      EnemySpawnPoint spawner = InstantiateRegistered(AssetPaths.EnemySpawnerPath, at)
        .GetComponent<EnemySpawnPoint>();
      ILootSpawner lootSpawner = spawner.gameObject.GetComponent<ILootSpawner>();
      lootSpawner.Construct(this, spawnerId, enemyTypeId);
      spawner.Construct(this, spawnerId, enemyTypeId, lootSpawner);
    }

    private GameObject InstantiateLoot(EnemyTypeId typeId, Vector3 position)
    {
      EnemyStaticData lootData = _enemyDataService.ForEnemy(typeId);
      GameObject lootObject = InstantiateFromPrefab(lootData.LootPrefab);
      ILoot loot = lootObject.GetComponent<ILoot>();
      loot.Souls = _randomService.Range(lootData.SoulsMin, lootData.SoulsMax, true);
      lootObject.transform.position = position;
      return lootObject;
    }

    private GameObject InitializePlayerComponents(GameObject player)
    {
      IAnimator playerAnimator = player.GetComponent<IAnimator>();

      IPlayerMetadata playerMetadata = player.GetComponent<IPlayerMetadata>();
      playerMetadata.Construct(_staticDataService.GameConfig);

      IHealth playerHealth = player.GetComponent<IHealth>();
      playerHealth.Construct(playerAnimator);

      IDeath playerDeath = player.GetComponent<IDeath>();
      playerDeath.Construct(playerAnimator, playerHealth);

      IPlayerAttacker playerAttack = player.GetComponent<IPlayerAttacker>();
      playerAttack.Construct(
        _inputService,
        _timeService,
        _staticDataService.GameConfig,
        _staticDataService.BuildConfig,
        playerAnimator);

      IPlayerMove playerMove = player.GetComponent<IPlayerMove>();
      playerMove.Construct(_inputService, _timeService, playerAttack);

      return player;
    }

    private GameObject InstantiateEnemy(EnemyTypeId typeId, Transform parent)
    {
      EnemyStaticData enemyData = _enemyDataService.ForEnemy(typeId);

      GameObject enemy = Object.Instantiate(enemyData.Prefab, parent);
      SetStaticData(enemy, enemyData);

      IAnimator enemyAnimator = enemy.GetComponent<IAnimator>();

      IEnemyHealth enemyHealth = enemy.GetComponent<IEnemyHealth>();
      enemyHealth.Construct(enemyAnimator);
      IEnemyHurtboxMetadata enemyHurtboxMetadata =
        enemy.GetComponentInChildren<IEnemyHurtboxMetadata>();
      enemyHurtboxMetadata.Construct(_staticDataService.GameConfig);

      IEnemyDeath enemyDeath = enemy.GetComponent<IEnemyDeath>();
      enemyDeath.Construct(enemyAnimator, enemyHealth);

      IEnemyAttacker enemyAttacker = enemy.GetComponent<IEnemyAttacker>();
      GameObject player = _playerReader.GetPlayer();
      IDeath playerDeath = player.GetComponent<IDeath>();
      IHealth playerHealth = player.GetComponent<IHealth>();
      enemyAttacker.Construct(
        player, enemyAnimator, playerDeath, playerHealth, enemyHealth,
        _buildConfig, _gameConfig
        );

      ICheckAttackRange checkAttackRange = enemy.GetComponent<ICheckAttackRange>();
      checkAttackRange.Construct(enemyAttacker);
      IAttackZoneMetadata enemyAttackZoneMetadata =
        enemy.GetComponentInChildren<IAttackZoneMetadata>();
      enemyAttackZoneMetadata.Construct(_staticDataService.GameConfig);

      IMovableAgent enemyMovable = enemy.GetComponent<IMovableAgent>();
      enemyMovable.Construct(_playerReader, enemyAttacker);

      IAggro aggro = enemy.GetComponent<IAggro>();
      aggro.Construct(enemyMovable);
      IAggroMetadata aggroMetadata =
        enemy.GetComponentInChildren<IAggroMetadata>();
      aggroMetadata.Construct(_staticDataService.GameConfig);

      return enemy;
    }

    private void SetStaticData(GameObject enemy, EnemyStaticData enemyData)
    {
      foreach (var receiver in enemy.GetComponentsInChildren<IEnemyStaticDataReceiver>())
      {
        receiver.SetValues(enemyData);
      }
    }

    /// <summary>
    /// Object needs manual placement after in this case.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>GameObject</returns>
    private GameObject InstantiateRegistered(string path)
    {
      GameObject prefab = _assetProvider.LoadAsset(path);
      GameObject gameobject = Object.Instantiate(prefab);
      RegisterProgressWatchers(gameobject);
      return gameobject;
    }

    /// <summary>
    /// Overload that allows immediate placement after instantiation.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="at"></param>
    /// <returns>GameObject</returns>
    private GameObject InstantiateRegistered(string path, Vector3 at)
    {
      GameObject gameObject = InstantiateRegistered(path);
      gameObject.transform.position = at;
      return gameObject;
    }

    private GameObject InstantiateFromPrefab(GameObject prefab)
    {
      GameObject gameobject = Object.Instantiate(prefab);
      RegisterProgressWatchers(gameobject);
      return gameobject;
    }

    private void RegisterProgressWatchers(GameObject gameObject)
    {
      foreach (IProgressWatcher progressIO in
        gameObject.GetComponentsInChildren<IProgressWatcher>())
      {
        Register(progressIO);
      }
    }

    private void Register(IProgressWatcher progressIO)
    {
      if (progressIO is IProgressReader progressReader)
      {
        ProgressReaders.Add(progressReader);
      }

      if (progressIO is IProgressWriter progressWriter)
      {
        ProgressWriters.Add(progressWriter);
      }
    }

    private GameObject PlacePlayer(GameObject player)
    {
      var playerStart = GameObject.FindWithTag(_playerStartTag);

      if (playerStart == null)
      {
        _logger.Log(LogType.Warning,
          "PlayerStart not found. " +
          "Player was placed at scene zero coordinates with default transforms.");
        return player;
      }

      player.transform.position = playerStart.transform.position;
      player.transform.rotation = playerStart.transform.rotation;

      return player;
    }
  }
}
