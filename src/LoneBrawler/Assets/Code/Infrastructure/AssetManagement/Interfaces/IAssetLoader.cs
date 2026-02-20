// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.AssetManagement.Interfaces
{
  public interface IAssetLoader
  {
    void Intitialize();

    UniTask<T> LoadAsync<T>(AssetReference assetReference) where T : class;
    UniTask<T> LoadAsync<T>(string assetAddress) where T : class;

    public GameObject Load(string path);
    public T Load<T>(string path) where T : Object;

    UniTask<GameObject> InstantiateAsync(string address);
    UniTask<GameObject> InstantiateAsync(string address, Transform parent);
    UniTask<GameObject> InstantiateAsync(AssetReference assetReference);
    UniTask<GameObject> InstantiateAsync(AssetReference assetReference, Transform parent);

    void Cleanup();
  }
}
