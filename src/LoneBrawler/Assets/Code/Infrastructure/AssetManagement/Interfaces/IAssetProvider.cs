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
    Task<T> LoadAsync<T>(AssetReference assetReference) where T : class;
    public GameObject Load(string path);
    public T Load<T>(string path) where T : Object;
    void Cleanup();
  }
}
