// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Shop
{
  /// <summary>
  /// Spawner for shop items. Must be placed on a GameObject with LayoutGroup.
  /// </summary>
  public class ShopItemSpawner : ZenjexBehaviour
  {
    [Header("Settings")]
    [Tooltip("Список элементов магазина для спавна")]
    [SerializeField] private List<BuffClassName> itemsToSpawn = new();

    [Zenjex] private readonly IShopItemFactory _shopItemFactory;

    private readonly List<GameObject> _spawnedItems = new();

    private void Start() => SpawnItemsAsync().Forget();

    private async UniTaskVoid SpawnItemsAsync()
    {
      foreach (var itemId in itemsToSpawn)
      {
        if (itemId == BuffClassName.None)
          continue;

        GameObject itemObject = await _shopItemFactory.CreateShopItem(itemId, transform);
        
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
