// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class WindowDataSubservice : IWindowDataSubservice
  {
    private WindowsManifestStaticData _manifest;

    private DictionaryData<WindowTypeId, WindowStaticData> _loadedEnemies = new();

    private IAssetLoader _assetLoader;

    public WindowDataSubservice(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    public async UniTask LoadSelfAsync() =>
      _manifest = await _assetLoader
      .LoadAsync<WindowsManifestStaticData>(StaticDataAddresses.WindowsManifestAddress);

    public async UniTask<WindowStaticData> ForWindowAsync(WindowTypeId typeId)
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
