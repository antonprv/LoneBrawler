// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Threading.Tasks;

using Code.Infrastructure.AssetManagement.Interfaces;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Infrastructure.AssetManagement
{
  public class AssetProvider : IAssetProvider
  {
    private readonly Dictionary<string, AsyncOperationHandle> _completedHandles =
      new Dictionary<string, AsyncOperationHandle>();

    private readonly Dictionary<string, List<AsyncOperationHandle>> _calledHandles =
      new Dictionary<string, List<AsyncOperationHandle>>();

    public async Task<T> LoadAsync<T>(AssetReference assetReference) where T : class
    {
      if (_completedHandles.TryGetValue(
          assetReference.AssetGUID, out AsyncOperationHandle completeHandle) &&
          completeHandle.IsValid())
        return completeHandle.Result as T;

      return await RunWithCacheOnComplete(
        assetReference.AssetGUID,
        Addressables.LoadAssetAsync<T>(assetReference)
        );
    }

    public async Task<T> LoadAsync<T>(string assetAddress) where T : class
    {
      if (_completedHandles.TryGetValue(
        assetAddress, out AsyncOperationHandle completeHandle) && completeHandle.IsValid())
        return completeHandle.Result as T;

      return await RunWithCacheOnComplete(
        assetAddress,
        Addressables.LoadAssetAsync<T>(assetAddress)
        );
    }

    public GameObject Load(string path) => Resources.Load<GameObject>(path);

    public T Load<T>(string path) where T : Object => Resources.Load<T>(path);

    public void Cleanup()
    {
      foreach (List<AsyncOperationHandle> handles in _calledHandles.Values)
        foreach (AsyncOperationHandle handle in handles)
        {
          if (handle.IsValid())
            Addressables.Release(handle);
        }
    }

    private async Task<T> RunWithCacheOnComplete<T>(
      string key, AsyncOperationHandle<T> operationHandle) where T : class
    {
      operationHandle.Completed += cacheLambda =>
        _completedHandles[key] = cacheLambda;

      AddCalledHandle(key, operationHandle);

      return await operationHandle.Task;
    }

    private void AddCalledHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
    {
      if (!_calledHandles.TryGetValue(key, out List<AsyncOperationHandle> value))
      {
        value = new List<AsyncOperationHandle>();
        _calledHandles[key] = value;
      }
      value.Add(handle);
    }
  }
}
