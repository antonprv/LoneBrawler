using UnityEngine;
// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Services.AssetManagement
{
  public interface IAssetProvider
  {
    public GameObject LoadAsset(string path);
    public T LoadAsset<T>(string path) where T : Object;
  }
}
