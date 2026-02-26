// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class ShopItemDataSubservice : IShopItemDataSubservice
  {
    private ShopItemsManifestStaticData _manifest;

    private readonly DictionaryData<BuffClassName, ShopItemStaticData> _loadedItems = new();

    private readonly IAssetLoader _assetLoader;

    public ShopItemDataSubservice(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    public async UniTask LoadSelfAsync() =>
      _manifest =
        await _assetLoader
        .LoadAsync<ShopItemsManifestStaticData>(StaticDataAddresses.ShopItemsManifestAddress);

    public async UniTask<ShopItemStaticData> ForShopItemAsync(BuffClassName className)
    {
      if (_loadedItems.TryGetValue(className, out ShopItemStaticData cached))
        return cached;

      _manifest.ShopItems.TryGetValue(className, out AssetReferenceT<ShopItemStaticData> entry);
      if (entry == null) return null;

      ShopItemStaticData data = await _assetLoader.LoadAsync<ShopItemStaticData>(entry);

      _loadedItems[className] = data;
      return data;
    }
  }
}
