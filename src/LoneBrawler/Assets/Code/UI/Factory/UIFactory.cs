// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Data.StaticData.Types;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.UI.Factory.Interfaces;
using Code.UI.Windows;

using UnityEngine;

namespace Code.UI.Factory
{
  internal class UIFactory : IUIFactory
  {
    private IAssetLoader _assetLoader;
    private IStaticDataService _staticData;
    private Transform _uiRoot;
    private IPersistentProgressService _persistentProgress;
    private GameObject _uiRootPrefab;

    public UIFactory()
    {
      _assetLoader = RootContext.Resolve<IAssetLoader>();
      _staticData = RootContext.Resolve<IStaticDataService>();
      _persistentProgress = RootContext.Resolve<IPersistentProgressService>();
    }

    public async Task WarmUp() =>
      _uiRootPrefab = await _assetLoader.LoadAsync<GameObject>(AssetAddresses.UIRootAddress);

    public async void CreateShop(WindowTypeId typeId)
    {
      WindowStaticData windowData = await _staticData.WindowData.ForWindowAsync(typeId);
      GameObject windowObject = await _assetLoader.InstantiateAsync(windowData.WindowReference, _uiRoot);

      WindowBase window = windowObject.GetComponent<WindowBase>();

      window.Construct(_persistentProgress);
    }

    public void CreateUIRootAsync() =>
      _uiRoot = GameObject.Instantiate(_uiRootPrefab).transform;

    public void Cleanup() => _assetLoader.Cleanup();
  }
}
