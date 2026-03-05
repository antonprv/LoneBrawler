// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.UI.Factory.Interfaces;
using Code.UI.Windows;
using Code.UI.Windows.Types;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Factory
{
  public class UIFactory : IUIFactory
  {
    public HashSet<WindowTypeId> OpenWindows => _openWindows;

    private readonly IAssetLoader _assetLoader;
    private readonly IStaticDataService _staticData;
    private readonly IPersistentProgressService _persistentProgress;

    private Transform _uiRoot;
    private GameObject _uiRootPrefab;
    private readonly HashSet<WindowTypeId> _openWindows = new();

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

    public async UniTask WarmUp() =>
      _uiRootPrefab = await _assetLoader.LoadAsync<GameObject>(AssetAddresses.UIRootAddress);

    public async UniTask CreateMainMenuAsync(Button openButton = null, ConstructorContext context = ConstructorContext.InCode) =>
      await CreateWindow(WindowTypeId.MainMenu, openButton, context);

    public async UniTask CreateWindow(
      WindowTypeId typeId, Button openButton, ConstructorContext context = ConstructorContext.InCode)
    {
      if (OpenWindows.Contains(typeId)) return;

      WindowStaticData windowData = await _staticData.WindowData.ForWindowAsync(typeId);
      GameObject windowObject = await _assetLoader.InstantiateAsync(windowData.WindowReference, _uiRoot);

      WindowBase window = windowObject.GetComponent<WindowBase>();

      window.Construct(_persistentProgress, context, openButton);

      OpenWindows.Add(typeId);
    }

    public void CreateUIRootAsync() =>
      _uiRoot = GameObject.Instantiate(_uiRootPrefab).transform;

    public void Cleanup()
    {
      OpenWindows.Clear();
      _assetLoader.Cleanup();
    }
  }
}
