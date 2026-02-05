// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Factory.Interfaces;

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData.Types;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.UI.Types;
using Code.UI.Windows;

using UnityEngine;

namespace Code.UI.Factory
{
  internal class UIFactory : IUIFactory
  {
    private IAssetProvider _assetProvider;
    private IStaticDataService _staticData;
    private Transform _uiRoot;
    private IPersistentProgressService _persistentProgress;

    public UIFactory()
    {
      _assetProvider = RootContext.Resolve<IAssetProvider>();
      _staticData = RootContext.Resolve<IStaticDataService>();
      _persistentProgress = RootContext.Resolve<IPersistentProgressService>();
    }

    public void CreateShop(WindowTypeId typeId)
    {
      WindowConfig _config = _staticData.WindowData.ForWindow(typeId);
      WindowBase window = Object.Instantiate(_config.windowPrefab, _uiRoot);
      window.Construct(_persistentProgress);
    }

    public void CreateUIRoot() =>
      _uiRoot = Object.Instantiate(
        _assetProvider.LoadAsset(AssetPaths.UIRootPath).transform
        );
  }
}
