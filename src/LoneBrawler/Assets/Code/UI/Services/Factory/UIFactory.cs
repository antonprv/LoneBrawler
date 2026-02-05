// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData.Types;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.UI.Services.Factory.Interfaces;
using Code.UI.Types;

using UnityEngine;

namespace Code.UI.Services.Factory
{
  internal class UIFactory : IUIFactory
  {
    private IAssetProvider _assetProvider;
    private IStaticDataService _staticData;
    private Transform _uiRoot;

    public UIFactory()
    {
      _assetProvider = RootContext.Resolve<IAssetProvider>();
      _staticData = RootContext.Resolve<IStaticDataService>();
    }

    public void CreateShop(WindowTypeId typeId)
    {
      WindowConfig _config = _staticData.WindowData.ForWindow(typeId);
      Object.Instantiate(_config.windowPrefab, _uiRoot);
    }

    public void CreateUIRoot() =>
      _uiRoot = Object.Instantiate(
        _assetProvider.LoadAsset(AssetPaths.UIRootPath).transform
        );
  }
}
