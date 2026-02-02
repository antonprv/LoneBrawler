// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Assets.Code.Gameplay.Features.Enemies.DataReceiver;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Data.StaticData.DataReceivers;
using Code.Gameplay.Common.NPCInterfaces.Animations;
using Code.Gameplay.Common.NPCInterfaces.DamageSystem;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Enemies.Attack.Interfaces;
using Code.Gameplay.Features.Enemies.DataReceiver.Interfaces;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Gameplay.Features.Enemies.Movement.Interfaces;
using Code.Gameplay.Features.Loot;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

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

      _enemyDataService = _staticDataService.EnemyData;
      _buildConfig = _staticDataService.BuildConfig;
      _gameConfig = _staticDataService.GameConfig;
      _playerStartTag = _gameConfig.PlayerStartTag;
    }

    public List<IProgressReader> ProgressReaders { get; } = new List<IProgressReader>();
    public List<IProgressWriter> ProgressWriters { get; } = new List<IProgressWriter>();

    /*-----------------public API-----------------------*/

    public void RegisterExternal(GameObject gameObject) =>
      RegisterProgressWatchers(gameObject);

    public GameObject CreatePlayer() =>
      InitializePlayerComponents(
        InstantiateRegistered(AssetPaths.PlayerPath)
        );

    public GameObject CreateAndPlacePlayer() =>
      InitializePlayerComponents(
        PlacePlayer(player: InstantiateRegistered(AssetPaths.PlayerPath))
        );

    public GameObject CreateHud() =>
      InstantiateRegistered(AssetPaths.HudPath);

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

    private GameObject InstantiateLoot(EnemyTypeId typeId, Vector3 position)
    {
      EnemyStaticData lootData = _enemyDataService.ForEnemy(typeId);
      GameObject lootObject = InstantiateFromPrefab(lootData.LootPrefab);
      ILoot loot = lootObject.GetComponent<ILoot>();
      loot.Souls = _randomService.Range(lootData.SoulsMin, lootData.SoulsMax, true);
      lootObject.transform.position = position;
      return lootObject;
    }

    private static GameObject InitializePlayerComponents(GameObject player)
    {
      IAnimator playerAnimator = player.GetComponent<IAnimator>();

      IHealth playerHealth = player.GetComponent<IHealth>();
      playerHealth.Construct(playerAnimator);

      IDeath playerDeath = player.GetComponent<IDeath>();
      playerDeath.Construct(playerAnimator, playerHealth);

      return player;
    }

    private GameObject InstantiateEnemy(EnemyTypeId typeId, Transform parent)
    {
      EnemyStaticData enemyData = _enemyDataService.ForEnemy(typeId);

      GameObject enemy = Object.Instantiate(enemyData.Prefab, parent);
      ReceiveStaticData(enemy, enemyData);

      IAnimator enemyAnimator = enemy.GetComponent<IAnimator>();

      IEnemyHealth enemyHealth = enemy.GetComponent<IEnemyHealth>();
      enemyHealth.Construct(enemyAnimator);

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

      IMovableAgent enemyMovable = enemy.GetComponent<IMovableAgent>();
      enemyMovable.Construct(_playerReader, enemyAttacker);

      return enemy;
    }

    private void ReceiveStaticData(GameObject enemy, EnemyStaticData enemyData)
    {
      foreach (var receiver in enemy.GetComponentsInChildren<IEnemyStaticDataReceiver>())
      {
        receiver.SetValues(enemyData);
      }
    }

    private GameObject InstantiateRegistered(string path)
    {
      GameObject prefab = _assetProvider.LoadAsset(path);
      GameObject gameobject = Object.Instantiate(prefab);
      RegisterProgressWatchers(gameobject);
      return gameobject;
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
