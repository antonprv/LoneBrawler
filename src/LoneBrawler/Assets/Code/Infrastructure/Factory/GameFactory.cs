// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
  public class GameFactory : IGameFactory
  {
    private readonly IAssetProvider _assetProvider;

    public GameFactory()
    {
      _assetProvider = RootContext.Resolve<IAssetProvider>();
    }

    public GameObject CreateHero()
    {
      GameObject heroPrefab = _assetProvider.LoadAsset(AssetPaths.HeroPath);
      return Object.Instantiate(heroPrefab);
    }

    public GameObject CreateAndPlaceHero()
    {
      GameObject heroPrefab = _assetProvider.LoadAsset(AssetPaths.HeroPath);
      PlaceHero(heroPrefab);
      return Object.Instantiate(heroPrefab);
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
