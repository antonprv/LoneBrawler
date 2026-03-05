// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Data.StaticData.Types.Buff;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.UI.Elements.Shop
{
  [Serializable]
  public class BuffClassNameEntry
  {
    [FilteredEnum(BuffClassName.None, BuffClassName.BuffBase)]
    public BuffClassName Value;
  }

  /// <summary>
  /// Spawner for shop items. Must be placed on a GameObject with LayoutGroup.
  /// </summary>
  public class ShopItemSpawner : MonoBehaviour
  {
    [Header("Settings")]
    public List<BuffClassNameEntry> itemsToSpawn = new();

    private IShopItemFactory _shopItemFactory;

    private readonly List<GameObject> _spawnedItems = new();

    public void Construct()
    {
      InjectDependencies();
    }

    private void InjectDependencies() =>
      _shopItemFactory = RootContext.Resolve<IShopItemFactory>();

    public void SpawnItems() => SpawnItemsAsync().Forget();

    private async UniTaskVoid SpawnItemsAsync()
    {
      foreach (var itemId in itemsToSpawn)
      {
        if (itemId.Value == BuffClassName.None)
          continue;

        GameObject itemObject = await _shopItemFactory.CreateShopItem(itemId.Value, transform);

        if (itemObject != null)
          _spawnedItems.Add(itemObject);
      }
    }

    private void OnDestroy()
    {
      foreach (var item in _spawnedItems)
        if (item != null)
          Destroy(item);

      _spawnedItems.Clear();
    }
  }
}
