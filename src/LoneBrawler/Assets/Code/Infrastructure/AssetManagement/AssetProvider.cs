// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
  public class AssetProvider : IAssetProvider
  {
    public GameObject LoadAsset(string path)
    {
      return (GameObject)Resources.Load(path);
    }

    public T LoadAsset<T>(string path) where T : Object
    {
      return Resources.Load<T>(path);
    }
  }
}
