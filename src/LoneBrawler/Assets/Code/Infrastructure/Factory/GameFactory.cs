// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Factory;

using Reflex.Attributes;

using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
  public class GameFactory : IGameFactory
  {
    [Inject]
    private readonly IAssetProvider _assetProvider;

    public GameObject CreatePlayer()
    {
      GameObject playerPrefab = _assetProvider.LoadAsset(AssetPaths.PlayerPath);
      return Object.Instantiate(playerPrefab);
    }

    public GameObject CreateAndPlacePlayer()
    {
      GameObject playerPrefab = _assetProvider.LoadAsset(AssetPaths.PlayerPath);
      GameObject player = GameObject.Instantiate(playerPrefab);
      PlaceHero(player);
      return Object.Instantiate(player);
    }

    public GameObject CreateHud()
    {
      GameObject hudPrefab = _assetProvider.LoadAsset(AssetPaths.HudPath);
      return Object.Instantiate(hudPrefab);
    }

    private void PlaceHero(GameObject hero)
    {
      var playerStart = GameObject.FindWithTag(FactoryNames.PlayerStartTag);
      hero.transform.position = playerStart.transform.position;
      hero.transform.rotation = playerStart.transform.rotation;
    }
  }
}
