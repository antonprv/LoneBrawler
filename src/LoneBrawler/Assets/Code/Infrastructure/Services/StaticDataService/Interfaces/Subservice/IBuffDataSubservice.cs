// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;

using Cysharp.Threading.Tasks;

using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IBuffDataSubservice
  {
    UniTask<BuffStaticData> ForBuffAsync(BuffClassName buffClassKey);
    UniTask LoadSelfAsync();

    bool TryGet<T>(BuffStaticData data, string key, out T value);
    int GetInt(BuffStaticData data, string key, int fallback = 0);
    float GetFloat(BuffStaticData data, string key, float fallback = 0f);
    bool GetBool(BuffStaticData data, string key, bool fallback = false);
    string GetString(BuffStaticData data, string key, string fallback = "");
    AssetReference GetAssetReference(BuffStaticData data, string key, AssetReference fallback = null);
  }
}
