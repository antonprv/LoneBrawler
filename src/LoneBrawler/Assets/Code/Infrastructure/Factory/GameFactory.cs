// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEngine;

#region Dependency Injection Imports

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.Random;
using Code.Infrastructure.Services.Time;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.UI.Services.WindowService.Interfaces;

#endregion

#region Component Interfaces

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
using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Gameplay.LevelTeleport;
using Code.Infrastructure.Factory.Interfaces;
using Code.UI.Elements;
using Code.Gameplay.LevelTeleport.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using Code.Gameplay.Features.Enemies.Spawn;
using Code.Gameplay.Features.Enemies;
using Code.Gameplay.Save.Interfaces;

using Code.Gameplay.Utils.ActorComponents.Interfaces;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Common.CustomTypes.Infrastructure.Types;

using Zenjex.Extensions.Core;

using Cysharp.Threading.Tasks;

using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;

using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Attack;
using Code.Data.Metadata;

#endregion

namespace Code.Infrastructure.Factory
{
  public class GameFactory : IGameFactory
  {
    #region Dependencies

    private readonly IGameLog _logger;
    private readonly IAssetLoader _assetLoader;
    private readonly IStaticDataService _staticDataService;
    private readonly IPlayerReader _playerReader;
    private readonly IRandomService _randomService;
    private readonly IInputService _inputService;
    private readonly ITimeService _timeService;
    private readonly IWindowService _windowService;
    private readonly ILootTrackerService _lootTracker;
    private readonly IPersistentProgressService _progressService;
    private readonly IAttackBehaviourFactory _attackBehaviourFactory;
    private readonly IEnemyDataSubservice _enemyDataService;
    private readonly IBuildConfigSubservice _buildConfig;
    private readonly IGameConfigSubservice _gameConfig;

    private GameObject _levelTeleportPrefab;
    private GameObject _enemySpawnerPrefab;

    private ISaveLoadService __internalSaveLoad__;
    private ISaveLoadService SaveLoadService =>
    __internalSaveLoad__ ??= RootContext.Resolve<ISaveLoadService>();

    private IGameStateMachine __internalStateMachine__;
    private IGameStateMachine StateMachine =>
        __internalStateMachine__ ??= RootContext.Resolve<IGameStateMachine>();

    public List<IProgressReader> ProgressReaders { get; } = new List<IProgressReader>();
    public List<IProgressWriter> ProgressWriters { get; } = new List<IProgressWriter>();

    public GameFactory(
      IGameLog gameLog,
      IAssetLoader assetLoader,
      IStaticDataService staticDataService,
      IPlayerReader playerReader,
      IRandomService randomService,
      IInputService inputService,
      ITimeService timeService,
      IWindowService windowService,
      ILootTrackerService lootTrackerService,
      IPersistentProgressService persistentProgressService,
      IAttackBehaviourFactory attackBehaviourFactory
      )
    {
      _logger = gameLog;
      _assetLoader = assetLoader;

      _staticDataService = staticDataService;
      _playerReader = playerReader;
      _randomService = randomService;
      _inputService = inputService;
      _timeService = timeService;
      _windowService = windowService;
      _lootTracker = lootTrackerService;
      _progressService = persistentProgressService;
      _attackBehaviourFactory = attackBehaviourFactory;

      _enemyDataService = _staticDataService.EnemyData;
      _buildConfig = _staticDataService.BuildConfig;
      _gameConfig = _staticDataService.GameConfig;
    }

    #endregion

    #region Public API

    public async UniTask WarmUp()
    {
      (_levelTeleportPrefab, _enemySpawnerPrefab) = await UniTask.WhenAll(
          _assetLoader.LoadAsync<GameObject>(AssetAddresses.LevelTeleportAddress),
          _assetLoader.LoadAsync<GameObject>(AssetAddresses.EnemySpawnerAddress)
      );
    }

    public async UniTask<GameObject> CreatePlayerAsync() =>
      await InitializePlayerAsync();

    public async UniTask<GameObject> CreateAndPlacePlayerAsync(Coordinates at) =>
      await InitializePlayerAsync(at);

    public async UniTask<GameObject> CreateHudAsync() =>
      await InitializeHudAsync();

    public void CreateEnemySpawner(Vector3 at, string spawnerId, EnemyTypeId enemyTypeId) =>
      InitializeEnemySpawner(at, spawnerId, enemyTypeId);

    public async UniTask<GameObject> CreateEnemy(EnemyTypeId typeId, Transform parent) =>
      await InitializeEnemy(typeId, parent);

    public async UniTask<GameObject> CreateLoot(EnemyTypeId typeId, Vector3 position) =>
      await InitializeLoot(typeId, position);

    public void CreateTeleport(Coordinates coords, Vector3 scale, string levelKey, string uniqueName) =>
      InitializeLevelTeleport(coords, scale, levelKey, uniqueName);

    public void Cleanup()
    {
      ProgressReaders.Clear();
      ProgressWriters.Clear();
    }

    #endregion

    #region Player

    private async UniTask<GameObject> InitializePlayerAsync(Coordinates at = null)
    {
      GameObject player = await InstantiateAndRegisterAsync(AssetAddresses.PlayerAddress);

      player.SetActive(false);

      DeactivateAllComponentsOn(player);
      ConfigurePlayerComponents(player, at);

      await UniTask.Yield();
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
      IMetadata metadata = player.GetComponent<IMetadata>();
      metadata.AssignMetadata();
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
      attacker.Construct(animator);
      return attacker;
    }

    private void ConfigurePlayerMovement(GameObject player, IPlayerAttacker attacker)
    {
      IPlayerMove movement = player.GetComponent<IPlayerMove>();
      movement.Construct(attacker);
    }

    #endregion

    #region Enemy

    private async UniTask<GameObject> InitializeEnemy(EnemyTypeId typeId, Transform parent)
    {
      EnemyStaticData enemyData = await _enemyDataService.ForEnemyAsync(typeId);

      GameObject enemy =
        await _assetLoader.InstantiateAsync(enemyData.PrefabReference, parent);
      enemy.SetActive(false);

      GameObject player = _playerReader.GetPlayer();
      IHealth playerHealth = player.GetComponent<IHealth>();

      IAttackBehaviour attackBehaviour =
        await CreateAttackBehaviour(enemyData, enemy, playerHealth);

      ApplyStaticDataToEnemy(enemy, enemyData);
      DeactivateAllComponentsOn(enemy);
      ConfigureEnemyComponents(enemy, attackBehaviour);

      await UniTask.Yield();
      ActivateAllComponentsOn(enemy);

      enemy.SetActive(true);
      RunManualStartOn(enemy);

      return enemy;
    }

    private async UniTask<IAttackBehaviour> CreateAttackBehaviour(
      EnemyStaticData enemyData,
      GameObject enemy,
      IHealth playerHealth
      )
    {
      AttackPresetStaticData attackPreset =
        await _enemyDataService.ForAttackPresetAsync(enemyData);

      IAttackBehaviour attackBehaviour = await _attackBehaviourFactory.CreateAsync(
        ownerTransform: enemy.transform,
        staticData: enemyData,
        preset: attackPreset,
        playerHealth: playerHealth,
        playerLayerMask: _gameConfig.PlayerLayerBitmask,
        assetLoader: _assetLoader);
      return attackBehaviour;
    }

    // ConfigureEnemyComponents receives behavior as a parameter
    private void ConfigureEnemyComponents(GameObject enemy, IAttackBehaviour attackBehaviour)
    {
      IAnimator animator = enemy.GetComponent<IAnimator>();

      IHealth health = ConfigureEnemyHealth(enemy, animator);
      ConfigureEnemyDeath(enemy, animator, health);
      IEnemyAttacker attacker = ConfigureEnemyAttack(enemy, animator, health, attackBehaviour);
      ConfigureEnemyAttackRange(enemy, attacker);
      IMovableAgent movement = ConfigureEnemyMovement(enemy, attacker);
      ConfigureEnemyAggro(enemy, movement);
      ConfigureEnemyMetadata(enemy);
    }

    private void ConfigureEnemyMetadata(GameObject enemy)
    {
      foreach (var metadata in enemy.GetComponentsInChildren<IMetadata>())
        metadata.AssignMetadata();
    }

    private IHealth ConfigureEnemyHealth(GameObject enemy, IAnimator animator)
    {
      IHealth health = enemy.GetComponent<IHealth>();
      health.Construct(animator);
      return health;
    }

    private void ConfigureEnemyDeath(GameObject enemy, IAnimator animator, IHealth health)
    {
      IEnemyDeath death = enemy.GetComponent<IEnemyDeath>();
      death.Construct(animator, health);
    }

    private IEnemyAttacker ConfigureEnemyAttack(
      GameObject enemy,
      IAnimator animator,
      IHealth enemyHealth,
      IAttackBehaviour attackBehaviour)
    {
      IEnemyAttacker attacker = enemy.GetComponent<IEnemyAttacker>();
      GameObject player = _playerReader.GetPlayer();

      IDeath playerDeath = player.GetComponent<IDeath>();
      IHealth playerHealth = player.GetComponent<IHealth>();

      attacker.Construct(player, animator, playerDeath, playerHealth, enemyHealth, _buildConfig, _gameConfig);

      // Pass a ready-made strategy — EnemyAttack knows nothing about the type of attack
      if (attacker is EnemyAttack enemyAttack)
        enemyAttack.SetAttackBehaviour(attackBehaviour);

      return attacker;
    }

    private void ConfigureEnemyAttackRange(GameObject enemy, IEnemyAttacker attacker)
    {
      ICheckAttackRange attackRange = enemy.GetComponent<ICheckAttackRange>();
      attackRange.Construct(attacker);
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
    }

    private void ApplyStaticDataToEnemy(GameObject enemy, EnemyStaticData enemyData)
    {
      foreach (IEnemyStaticDataReceiver receiver in enemy.GetComponentsInChildren<IEnemyStaticDataReceiver>())
        receiver.SetValues(enemyData);
    }

    #endregion

    #region Loot

    private async UniTask<GameObject> InitializeLoot(EnemyTypeId typeId, Vector3 position)
    {
      EnemyStaticData enemyData = await _enemyDataService.ForEnemyAsync(typeId);

      GameObject lootObject = await _assetLoader.InstantiateAsync(enemyData.LootPrefabReference);

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

      IMetadata lootMetadata = lootObject.GetComponentInChildren<IMetadata>();
      lootMetadata.AssignMetadata();
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
      spawnerMetadata.AssignMetadata();
    }

    #endregion

    #region UI

    private async UniTask<GameObject> InitializeHudAsync()
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
        stateMachine: StateMachine,
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

    private async UniTask<GameObject> InstantiateAndRegisterAsync(string assetReference)
    {
      GameObject instance = await _assetLoader.InstantiateAsync(assetReference);
      RegisterProgressWatchers(instance);
      return instance;
    }

    private async UniTask<GameObject> InstantiateAndRegisterAsync(string assetReference, Vector3 position)
    {
      GameObject instance = await _assetLoader.InstantiateAsync(assetReference);
      instance.transform.position = position;
      RegisterProgressWatchers(instance);
      return instance;
    }

    private async UniTask<GameObject> InstantiateAndRegisterAsync(string assetReference, Transform parent)
    {
      GameObject instance = await _assetLoader.InstantiateAsync(assetReference, parent);
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
      GameObject prefab = _assetLoader.Load(assetPath);
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
