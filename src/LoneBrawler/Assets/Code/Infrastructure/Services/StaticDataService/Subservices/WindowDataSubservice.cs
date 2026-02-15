// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.CustomTypes.Types;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class WindowDataSubservice : IWindowDataSubservice
  {
    private WindowsManifestStaticData _manifest;

    private DictionaryData<WindowTypeId, WindowStaticData> _loadedEnemies = new();

    private IAssetLoader _assetLoader;

    public WindowDataSubservice() => _assetLoader = RootContext.Resolve<IAssetLoader>();

    public async Task LoadSelfAsync() =>
      _manifest = await _assetLoader
      .LoadAsync<WindowsManifestStaticData>(StaticDataAddresses.WindowsManifestAddress);

    public async Task<WindowStaticData> ForWindowAsync(WindowTypeId typeId)
    {
      if (_loadedEnemies.TryGetValue(typeId, out WindowStaticData cached))
        return cached;

      if (!_manifest.Windows.TryGetValue(typeId, out AssetReferenceT<WindowStaticData> entry))
        return null;

      WindowStaticData data = await _assetLoader.LoadAsync<WindowStaticData>(entry);

      _loadedEnemies[typeId] = data;
      return data;
    }
  }
}
