// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.UI.Factory.Interfaces;
using Code.UI.Windows;
using Code.UI.Windows.Types;

using UnityEngine;

namespace Code.UI.Factory
{
  internal class UIFactory : IUIFactory
  {
    private readonly IAssetLoader _assetLoader;
    private readonly IStaticDataService _staticData;
    private readonly IPersistentProgressService _persistentProgress;

    private Transform _uiRoot;
    private GameObject _uiRootPrefab;

    public UIFactory(
      IAssetLoader assetLoader,
      IStaticDataService staticDataService,
      IPersistentProgressService persistentProgressService
      )
    {
      _assetLoader = assetLoader;
      _staticData = staticDataService;
      _persistentProgress = persistentProgressService;
    }

    public async Task WarmUp() =>
      _uiRootPrefab = await _assetLoader.LoadAsync<GameObject>(AssetAddresses.UIRootAddress);

    public async Task CreateMainMenuAsync(ConstructorContext context = ConstructorContext.InCode) =>
      await CreateWindow(WindowTypeId.MainMenu, context);

    public async Task CreateWindow(WindowTypeId typeId, ConstructorContext context = ConstructorContext.InCode)
    {
      WindowStaticData windowData = await _staticData.WindowData.ForWindowAsync(typeId);
      GameObject windowObject = await _assetLoader.InstantiateAsync(windowData.WindowReference, _uiRoot);

      WindowBase window = windowObject.GetComponent<WindowBase>();

      window.Construct(_persistentProgress, context);
    }

    public void CreateUIRootAsync() =>
      _uiRoot = GameObject.Instantiate(_uiRootPrefab).transform;

    public void Cleanup() => _assetLoader.Cleanup();
  }
}
