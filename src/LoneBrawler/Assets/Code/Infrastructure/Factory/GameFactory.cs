// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Services.PersistentProgress;

using UnityEngine;

namespace Code.Infrastructure.Factory
{
  public class GameFactory : IGameFactory
  {
    private readonly IGameLog _logger;
    private readonly IAssetProvider _assetProvider;

    public GameFactory()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _assetProvider = RootContext.Resolve<IAssetProvider>();
    }

    public List<IProgressReader> ProgressReaders { get; } = new List<IProgressReader>();
    public List<IProgressWriter> ProgressWriters { get; } = new List<IProgressWriter>();


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
      return gameobject;
    }

    private void RegisterProgressWatchers(GameObject gameObject)
    {
      foreach (IProgressWatcher progressIO in gameObject.GetComponentsInChildren<IProgressWatcher>())
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
      var playerStart = GameObject.FindWithTag(FactoryNames.PlayerStartTag);

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
