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
  public class BuffDataSubservice : IBuffDataSubservice
  {
    private BuffsManifestStaticData _manifest;

    private readonly DictionaryData<BuffClassName, BuffStaticData> _loadedBuffs = new();

    private readonly IAssetLoader _assetLoader;

    public BuffDataSubservice(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    public async UniTask LoadSelfAsync() =>
      _manifest =
        await _assetLoader
        .LoadAsync<BuffsManifestStaticData>(StaticDataAddresses.BuffsManifestAddress);

    public async UniTask<BuffStaticData> ForBuffAsync(BuffClassName buffClassKey)
    {
      if (_loadedBuffs.TryGetValue(buffClassKey, out BuffStaticData cached))
        return cached;

      _manifest.Buffs.TryGetValue(buffClassKey, out AssetReferenceT<BuffStaticData> entry);
      if (entry == null) return null;

      BuffStaticData data = await _assetLoader.LoadAsync<BuffStaticData>(entry);

      _loadedBuffs[buffClassKey] = data;
      return data;
    }
  }
}
