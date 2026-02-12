// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.AssetManagement.Interfaces
{
  public interface IAssetProvider
  {
    void Intitialize();

    Task<T> LoadAsync<T>(AssetReference assetReference) where T : class;
    Task<T> LoadAsync<T>(string assetAddress) where T : class;

    public GameObject Load(string path);
    public T Load<T>(string path) where T : Object;

    Task<GameObject> InstantiateAsync(string address);
    Task<GameObject> InstantiateAsync(string address, Transform parent);
    Task<GameObject> InstantiateAsync(AssetReference assetReference);
    Task<GameObject> InstantiateAsync(AssetReference assetReference, Transform parent);

    void Cleanup();
  }
}
