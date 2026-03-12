// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Infrastructure.AssetManagement
{
  public class AssetLoader : IAssetLoader
  {
    private readonly Dictionary<string, AsyncOperationHandle> _completedHandles = new();

    private readonly Dictionary<string, List<AsyncOperationHandle>> _calledHandles = new();

    private readonly List<AsyncOperationHandle<GameObject>> _instantiatedObjects = new();

    public async UniTask Intitialize() =>
      await Addressables.InitializeAsync().ToUniTask();

    public async UniTask<GameObject> InstantiateAsync(string address)
    {
      AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address);
      _instantiatedObjects.Add(handle);
      return await handle.ToUniTask();
    }

    public async UniTask<GameObject> InstantiateAsync(string address, Transform parent)
    {
      AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, parent);
      _instantiatedObjects.Add(handle);
      return await handle.ToUniTask();
    }

    public async UniTask<GameObject> InstantiateAsync(AssetReference assetReference)
    {
      AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetReference);
      _instantiatedObjects.Add(handle);
      return await handle.ToUniTask();
    }
    public async UniTask<GameObject> InstantiateAsync(AssetReference assetReference, Transform parent)
    {
      AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetReference, parent);
      _instantiatedObjects.Add(handle);
      return await handle.ToUniTask();
    }

    public async UniTask<T> LoadAsync<T>(AssetReference assetReference) where T : class
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

    public async UniTask<T> LoadAsync<T>(string assetAddress) where T : class
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
      foreach (AsyncOperationHandle<GameObject> handle in _instantiatedObjects)
      {
        if (handle.IsValid())
          Addressables.ReleaseInstance(handle);
      }
      _instantiatedObjects.Clear();

      foreach (List<AsyncOperationHandle> handles in _calledHandles.Values)
        foreach (AsyncOperationHandle handle in handles)
        {
          if (handle.IsValid())
            Addressables.Release(handle);
        }

      _calledHandles.Clear();
      _completedHandles.Clear();
    }

    private async UniTask<T> RunWithCacheOnComplete<T>(
      string key, AsyncOperationHandle<T> operationHandle) where T : class
    {
      operationHandle.Completed += cacheLambda =>
        _completedHandles[key] = cacheLambda;

      AddCalledHandle(key, operationHandle);

      return await operationHandle.ToUniTask();
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
