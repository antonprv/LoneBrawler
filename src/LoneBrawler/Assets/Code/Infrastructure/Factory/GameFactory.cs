// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Assets.Code.Gameplay.Features.Common;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Configs;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Services.PersistentProgress;

using UnityEngine;

namespace Code.Infrastructure.Factory
{
  public class GameFactory : IGameFactory
  {
    private readonly IGameLog _logger;
    private readonly IAssetProvider _assetProvider;
    private string _playerStartTag;

    public GameFactory()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _assetProvider = RootContext.Resolve<IAssetProvider>();

      _playerStartTag = GameConfiguration.PlayerStartTag;
    }

    public List<IProgressReader> ProgressReaders { get; } = new List<IProgressReader>();
    public List<IProgressWriter> ProgressWriters { get; } = new List<IProgressWriter>();
    public List<IConstructableComponent> InitializableComponents { get; } = new List<IConstructableComponent>();


    /*-----------------public API-----------------------*/

    public GameObject CreatePlayer() =>
      InstantiateRegistered(AssetPaths.PlayerPath);

    public GameObject CreateAndPlacePlayer() =>
      PlacePlayer(player: InstantiateRegistered(AssetPaths.PlayerPath));

    public GameObject CreateHud() => InstantiateRegistered(AssetPaths.HudPath);

    public void Cleanup()
    {
      ProgressReaders.Clear();
      ProgressWriters.Clear();
    }

    /*-----------------private methods------------------*/

    private GameObject InstantiateRegistered(string path)
    {
      GameObject prefab = _assetProvider.LoadAsset(path);
      GameObject gameobject = Object.Instantiate(prefab);
      RegisterProgressWatchers(gameobject);
      RegisterConstructableComponents(gameobject);
      return gameobject;
    }

    private void RegisterConstructableComponents(GameObject gameobject)
    {
      foreach (IConstructableComponent component in
        gameobject.GetComponentsInChildren<IConstructableComponent>())
      {
        RegisterComponent(component);
      }
    }

    private void RegisterComponent(IConstructableComponent component)
    {
      InitializableComponents.Add(component);
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
          "Hero was placed at Scene zero coordinates with default transforms.");
        return player;
      }

      player.transform.position = playerStart.transform.position;
      player.transform.rotation = playerStart.transform.rotation;

      return player;
    }
  }
}
