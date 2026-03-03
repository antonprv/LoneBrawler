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

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class BuffDataSubservice : IBuffDataSubservice
  {
    private BuffsManifestStaticData _manifest;

    private readonly DictionaryData<BuffClassName, BuffStaticData> _loadedBuffs = new();

    private readonly IAssetLoader _assetLoader;

    public BuffDataSubservice(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    #region General Static Data Interface

    public async UniTask LoadSelfAsync()
    {
      _manifest =
        await _assetLoader
        .LoadAsync<BuffsManifestStaticData>(StaticDataAddresses.BuffsManifestAddress);

      if (_manifest)
        Debug.Log($"[BuffDataSubservice]: Successfully loaded manifest." +
          $"Total {_manifest.Buffs.Count} buffs loaded.");
      else
        Debug.LogError($"[BuffDataSubservice]: Couldn't load manifest.");

    }

    public async UniTask<BuffStaticData> ForBuffAsync(BuffClassName buffClassKey)
    {
      if (_loadedBuffs.TryGetValue(buffClassKey, out BuffStaticData cached))
        return cached;

      if (!_manifest.Buffs.TryGetValue(buffClassKey, out AssetReferenceT<BuffStaticData> entry))
      {
        Debug.LogError($"[BuffDataSubservice] Buff '{buffClassKey}' not found in the buffs manifest");
        return null;
      }

      BuffStaticData data = await _assetLoader.LoadAsync<BuffStaticData>(entry);

      _loadedBuffs[buffClassKey] = data;
      return data;
    }

    #endregion

    #region Dynamic Parameter Accessors

    public bool TryGet<T>(BuffStaticData data, string key, out T value)
    {
      if (data.DynamicParameters.TryGetValue(key, out BuffParameterValue entry))
      {
        value = entry.Get<T>();
        return true;
      }

      value = default;
      return false;
    }

    public int GetInt(BuffStaticData data, string key, int fallback = 0) =>
      TryGet(data, key, out int v) ? v : fallback;

    public float GetFloat(BuffStaticData data, string key, float fallback = 0f) =>
      TryGet(data, key, out float v) ? v : fallback;

    public bool GetBool(BuffStaticData data, string key, bool fallback = false) =>
      TryGet(data, key, out bool v) ? v : fallback;

    public string GetString(BuffStaticData data, string key, string fallback = "") =>
      TryGet(data, key, out string v) ? v : fallback;

    public AssetReference GetAssetReference(BuffStaticData data, string key, AssetReference fallback = null) =>
      TryGet(data, key, out AssetReference v) ? v : fallback;

    #endregion
  }
}
