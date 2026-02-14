// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEngine;

#region Dependency Injection Imports

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.Random;
using Code.Infrastructure.Services.Time;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.UI.Services.WindowService.Interfaces;

#endregion

#region Component Interfaces

using Code.Data.StaticData;
using Code.Data.StaticData.DataReceivers;
using Code.Data.StaticData.Types;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Gameplay.Features.Enemies.Aggro.Interfaces;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Gameplay.Features.Player.Metadata.Interfaces;
using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Gameplay.LevelTeleport;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Factory.Interfaces;
using Code.UI.Elements;
using Code.Gameplay.LevelTeleport.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using Code.Gameplay.Features.Enemies.Spawn;
using Code.Gameplay.Features.Enemies;
using Code.Common.Extensions.CustomTypes.Types;
using Code.Gameplay.Save.Interfaces;

using System.Threading.Tasks;

using Code.Gameplay.Utils.ActorComponents.Interfaces;

#endregion

namespace Code.Infrastructure.Factory
{
  public class GameFactory : IGameFactory
  {
    #region Dependencies

    private readonly IGameLog _logger;
    private readonly IAssetProvider _assets;
    private readonly IStaticDataService _staticDataService;
    private readonly IPlayerReader _playerReader;
    private readonly IRandomService _randomService;
    private readonly IInputService _inputService;
    private readonly ITimeService _timeService;
    private readonly IWindowService _windowService;
    private readonly ILootTrackerService _lootTracker;
    private readonly IGameStateMachine _stateMachine;
    private readonly IPersistentProgressService _progressService;
    private readonly IEnemyDataSubservice _enemyDataService;
    private readonly IBuildConfigSubservice _buildConfig;
    private readonly IGameConfigSubservice _gameConfig;

    private ISaveLoadService __internalSaveLoad__;
    private GameObject _levelTeleportPrefab;
    private GameObject _enemySpawnerPrefab;

    private ISaveLoadService SaveLoadService =>
    __internalSaveLoad__ ??= RootContext.Resolve<ISaveLoadService>();

    public List<IProgressReader> ProgressReaders { get; } = new List<IProgressReader>();
    public List<IProgressWriter> ProgressWriters { get; } = new List<IProgressWriter>();

    public GameFactory()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _assets = RootContext.Resolve<IAssetProvider>();

      _staticDataService = RootContext.Resolve<IStaticDataService>();
      _playerReader = RootContext.Resolve<IPlayerReader>();
      _randomService = RootContext.Resolve<IRandomService>();
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();
      _windowService = RootContext.Resolve<IWindowService>();
      _lootTracker = RootContext.Resolve<ILootTrackerService>();
      _stateMachine = RootContext.Resolve<IGameStateMachine>();
      _progressService = RootContext.Resolve<IPersistentProgressService>();

      _enemyDataService = _staticDataService.EnemyData;
      _buildConfig = _staticDataService.BuildConfig;
      _gameConfig = _staticDataService.GameConfig;
    }

    #endregion

    #region Public API

    public async Task WarmUp()
    {
      Task<GameObject> levelTask = _assets.LoadAsync<GameObject>(AssetAddresses.LevelTeleportAddress);
      Task<GameObject> spawnerTask = _assets.LoadAsync<GameObject>(AssetAddresses.EnemySpawnerAddress);

      GameObject[] results = await Task.WhenAll(levelTask, spawnerTask);

      _levelTeleportPrefab = results[0];
      _enemySpawnerPrefab = results[1];
    }

    public async Task<GameObject> CreatePlayerAsync() =>
      await InitializePlayerAsync();

    public async Task<GameObject> CreateAndPlacePlayerAsync(Coordinates at) =>
      await InitializePlayerAsync(at);

    public async Task<GameObject> CreateHudAsync() =>
      await InitializeHudAsync();

    public void CreateEnemySpawner(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId) =>
      InitializeEnemySpawner(at, spawnerId, enemyTypeId);

    public async Task<GameObject> CreateEnemy(EnemyTypeId typeId, Transform parent) =>
      await InitializeEnemy(typeId, parent);

    public async Task<GameObject> CreateLoot(EnemyTypeId typeId, Vector3 position) =>
      await InitializeLoot(typeId, position);

    public void CreateTeleport(Coordinates coords, Vector3 scale, string levelKey, string uniqueName) =>
      InitializeLevelTeleport(coords, scale, levelKey, uniqueName);

    public void Cleanup()
    {
      ProgressReaders.Clear();
      ProgressWriters.Clear();

      _assets.Cleanup();
    }

    #endregion

    #region Player

    private async Task<GameObject> InitializePlayerAsync(Coordinates at = null)
    {
      GameObject player = await InstantiateAndRegisterAsync(AssetAddresses.PlayerAddress);

      player.SetActive(false);

      DeactivateAllComponentsOn(player);
      ConfigurePlayerComponents(player, at);

      await Task.Yield();
      ActivateAllComponentsOn(player);

      player.SetActive(true);

      RunManualStartOn(player);

      return player;
    }

    private void ConfigurePlayerComponents(GameObject player, Coordinates at)
    {
      IAnimator animator = player.GetComponent<IAnimator>();

      ConfigurePlayerMetadata(player);
      IHealth health = ConfigurePlayerHealth(player, animator);
      ConfigurePlayerDeath(player, animator, health);
      IPlayerAttacker attacker = ConfigurePlayerAttack(player, animator);
      ConfigurePlayerMovement(player, attacker);

      if (at != null)
        player.transform.SetPositionAndRotation(at.Position, at.Rotation);
    }

    private void ConfigurePlayerMetadata(GameObject player)
    {
      IPlayerMetadata metadata = player.GetComponent<IPlayerMetadata>();
      metadata.Construct(_gameConfig);
    }

    private IHealth ConfigurePlayerHealth(GameObject player, IAnimator animator)
    {
      IHealth health = player.GetComponent<IHealth>();
      health.Construct(animator);
      return health;
    }

    private void ConfigurePlayerDeath(GameObject player, IAnimator animator, IHealth health)
    {
      IDeath death = player.GetComponent<IDeath>();
      death.Construct(animator, health);
    }

    private IPlayerAttacker ConfigurePlayerAttack(GameObject player, IAnimator animator)
    {
      IPlayerAttacker attacker = player.GetComponent<IPlayerAttacker>();
      attacker.Construct(_inputService, _timeService, _gameConfig, _buildConfig, animator);
      return attacker;
    }

    private void ConfigurePlayerMovement(GameObject player, IPlayerAttacker attacker)
    {
      IPlayerMove movement = player.GetComponent<IPlayerMove>();
      movement.Construct(_inputService, _timeService, attacker);
    }

    #endregion

    #region Enemy

    private async Task<GameObject> InitializeEnemy(EnemyTypeId typeId, Transform parent)
    {
      EnemyStaticData enemyData = _enemyDataService.ForEnemy(typeId);
      GameObject enemy = await _assets.InstantiateAsync(enemyData.PrefabReference, parent);

      enemy.SetActive(false);

      ApplyStaticDataToEnemy(enemy, enemyData);
      DeactivateAllComponentsOn(enemy);
      ConfigureEnemyComponents(enemy);

      await Task.Yield();
      ActivateAllComponentsOn(enemy);

      enemy.SetActive(true);

      RunManualStartOn(enemy);

      return enemy;
    }

    private void ConfigureEnemyComponents(GameObject enemy)
    {
      IAnimator animator = enemy.GetComponent<IAnimator>();

      IEnemyHealth health = ConfigureEnemyHealth(enemy, animator);
      ConfigureEnemyDeath(enemy, animator, health);
      IEnemyAttacker attacker = ConfigureEnemyAttack(enemy, animator, health);
      ConfigureEnemyAttackRange(enemy, attacker);
      IMovableAgent movement = ConfigureEnemyMovement(enemy, attacker);
      ConfigureEnemyAggro(enemy, movement);
      ConfigureEnemyMetadata(enemy);
    }

    private void ConfigureEnemyMetadata(GameObject enemy)
    {
      EnemyMetadata enemyMetadata = enemy.GetComponent<EnemyMetadata>();
      enemyMetadata.Construct(_gameConfig);
    }

    private IEnemyHealth ConfigureEnemyHealth(GameObject enemy, IAnimator animator)
    {
      IEnemyHealth health = enemy.GetComponent<IEnemyHealth>();
      health.Construct(animator);

      IEnemyHurtboxMetadata hurtboxMetadata = enemy.GetComponentInChildren<IEnemyHurtboxMetadata>();
      hurtboxMetadata.Construct(_gameConfig);

      return health;
    }

    private void ConfigureEnemyDeath(GameObject enemy, IAnimator animator, IEnemyHealth health)
    {
      IEnemyDeath death = enemy.GetComponent<IEnemyDeath>();
      death.Construct(animator, health);
    }

    private IEnemyAttacker ConfigureEnemyAttack(GameObject enemy, IAnimator animator, IEnemyHealth enemyHealth)
    {
      IEnemyAttacker attacker = enemy.GetComponent<IEnemyAttacker>();
      GameObject player = _playerReader.GetPlayer();

      IDeath playerDeath = player.GetComponent<IDeath>();
      IHealth playerHealth = player.GetComponent<IHealth>();

      attacker.Construct(player, animator, playerDeath, playerHealth, enemyHealth, _buildConfig, _gameConfig);

      return attacker;
    }

    private void ConfigureEnemyAttackRange(GameObject enemy, IEnemyAttacker attacker)
    {
      ICheckAttackRange attackRange = enemy.GetComponent<ICheckAttackRange>();
      attackRange.Construct(attacker);

      IAttackZoneMetadata attackZoneMetadata = enemy.GetComponentInChildren<IAttackZoneMetadata>();
      attackZoneMetadata.Construct(_gameConfig);
    }

    private IMovableAgent ConfigureEnemyMovement(GameObject enemy, IEnemyAttacker attacker)
    {
      IMovableAgent movement = enemy.GetComponent<IMovableAgent>();
      movement.Construct(_playerReader, attacker);
      return movement;
    }

    private void ConfigureEnemyAggro(GameObject enemy, IMovableAgent movement)
    {
      IAggro aggro = enemy.GetComponent<IAggro>();
      aggro.Construct(movement);

      IAggroMetadata aggroMetadata = enemy.GetComponentInChildren<IAggroMetadata>();
      aggroMetadata.Construct(_gameConfig);
    }

    private void ApplyStaticDataToEnemy(GameObject enemy, EnemyStaticData enemyData)
    {
      foreach (IEnemyStaticDataReceiver receiver in enemy.GetComponentsInChildren<IEnemyStaticDataReceiver>())
        receiver.SetValues(enemyData);
    }

    #endregion

    #region Loot

    private async Task<GameObject> InitializeLoot(EnemyTypeId typeId, Vector3 position)
    {
      EnemyStaticData enemyData = _enemyDataService.ForEnemy(typeId);

      GameObject lootObject = await _assets.InstantiateAsync(enemyData.LootPrefabReference);

      lootObject.transform.position = position;

      ConfigureLootComponents(lootObject, enemyData);

      return lootObject;
    }

    private void ConfigureLootComponents(GameObject lootObject, EnemyStaticData enemyData)
    {
      ILoot loot = lootObject.GetComponent<ILoot>();
      loot.Souls = _randomService.Range(enemyData.SoulsMin, enemyData.SoulsMax, nonRepeating: true);

      ILootData lootData = lootObject.GetComponent<ILootData>();
      lootData.Construct(loot, _lootTracker);

      ILootMetadata lootMetadata = lootObject.GetComponentInChildren<ILootMetadata>();
      lootMetadata.Construct(_gameConfig);
    }

    #endregion

    #region Spawner

    private void InitializeEnemySpawner(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId)
    {
      GameObject spawnerObject = InstantiateAndRegister(_enemySpawnerPrefab, at);

      EnemySpawnPoint spawner = spawnerObject.GetComponent<EnemySpawnPoint>();
      ILootSpawner lootSpawner = spawnerObject.GetComponent<ILootSpawner>();

      spawner.Construct(this, spawnerId, enemyTypeId, lootSpawner);
      lootSpawner.Construct(this, spawnerId, enemyTypeId);

      EnemySpawnerMetadata spawnerMetadata = spawnerObject.GetComponent<EnemySpawnerMetadata>();
      spawnerMetadata.Construct(_gameConfig);
    }

    #endregion

    #region UI

    private async Task<GameObject> InitializeHudAsync()
    {
      GameObject hud = await InstantiateAndRegisterAsync(AssetAddresses.HudAddress);
      ConfigureHudButtons(hud);
      return hud;
    }

    private void ConfigureHudButtons(GameObject hud)
    {
      foreach (OpenWindowButton button in hud.GetComponentsInChildren<OpenWindowButton>())
        button.Construct(_windowService);
    }

    #endregion

    #region LevelTeleport

    private void InitializeLevelTeleport(
      Coordinates coords, Vector3 scale, string levelKey, string uniqueName)
    {
      GameObject teleportObject = Object.Instantiate(_levelTeleportPrefab);

      ISaveComponent saveComponent = teleportObject.GetComponent<ISaveComponent>();
      saveComponent.Construct(_logger, SaveLoadService);

      ILevelTeleportTriggerMetadata teleportMetadata =
        teleportObject.GetComponent<ILevelTeleportTriggerMetadata>();
      teleportMetadata.Construct(_gameConfig);

      LevelTeleportTrigger trigger = teleportObject.GetComponent<LevelTeleportTrigger>();
      trigger.Construct(
        progressService: _progressService,
        stateMachine: _stateMachine,
        timeService: _timeService,
        saveComponent: saveComponent,
        coords: coords,
        scale: scale,
        levelKey: levelKey,
        uniqueName: uniqueName
        );
    }

    #endregion

    #region Utilities

    private void RunManualStartOn(GameObject owner)
    {
      foreach (IManualStart component in owner.GetComponents<IManualStart>())
        component.ManualStart();
    }

    private void ActivateAllComponentsOn(GameObject owner)
    {
      foreach (IActivatable component in owner.GetComponents<IActivatable>())
        component.Activate();
    }

    private void DeactivateAllComponentsOn(GameObject owner)
    {
      foreach (IDeactivatable component in owner.GetComponents<IDeactivatable>())
        component.Deactivate();
    }

    #region Asynchronous Instantiation

    private async Task<GameObject> InstantiateAndRegisterAsync(string assetReference)
    {
      GameObject instance = await _assets.InstantiateAsync(assetReference);
      RegisterProgressWatchers(instance);
      return instance;
    }

    private async Task<GameObject> InstantiateAndRegisterAsync(string assetReference, Vector3 position)
    {
      GameObject instance = await _assets.InstantiateAsync(assetReference);
      instance.transform.position = position;
      RegisterProgressWatchers(instance);
      return instance;
    }

    private async Task<GameObject> InstantiateAndRegisterAsync(string assetReference, Transform parent)
    {
      GameObject instance = await _assets.InstantiateAsync(assetReference, parent);
      RegisterProgressWatchers(instance);
      return instance;
    }

    #endregion

    #region Synchronous Instantiation

    private GameObject InstantiateAndRegister(GameObject prefab)
    {
      GameObject instance = Object.Instantiate(prefab);
      RegisterProgressWatchers(instance);
      return instance;
    }

    private GameObject InstantiateAndRegister(GameObject prefab, Vector3 at)
    {
      GameObject instance = Object.Instantiate(prefab);
      instance.transform.position = at;
      RegisterProgressWatchers(instance);
      return instance;
    }

    private GameObject InstantiateAndRegister(string assetPath)
    {
      GameObject prefab = _assets.Load(assetPath);
      GameObject instance = Object.Instantiate(prefab);
      RegisterProgressWatchers(instance);
      return instance;
    }

    private GameObject InstantiateAndRegister(string assetPath, Vector3 position)
    {
      GameObject instance = InstantiateAndRegister(assetPath);
      instance.transform.position = position;
      return instance;
    }

    #endregion

    private void RegisterProgressWatchers(GameObject gameObject)
    {
      foreach (IProgressWatcher watcher in gameObject.GetComponentsInChildren<IProgressWatcher>())
        RegisterProgressWatcher(watcher);
    }

    private void RegisterProgressWatcher(IProgressWatcher watcher)
    {
      if (watcher is IProgressReader reader)
        ProgressReaders.Add(reader);

      if (watcher is IProgressWriter writer)
        ProgressWriters.Add(writer);
    }

    #endregion
  }
}
