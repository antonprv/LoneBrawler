// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Threading;

using Code.Data.StaticData.Types.Buff;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

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
  public class ShopItemSpawner : ZenjexBehaviour
  {
    [Header("Settings")]
    public List<BuffClassNameEntry> itemsToSpawn = new();

    [Zenjex] private readonly IShopItemFactory _shopItemFactory;

    private readonly List<GameObject> _spawnedItems = new();

    private CancellationToken _ct;

    protected override void OnAwake()
    {
      base.OnAwake();

      _ct = this.GetCancellationTokenOnDestroy();
    }

    public void SpawnItems() => SpawnItemsAsync().Forget();

    private async UniTaskVoid SpawnItemsAsync()
    {
      if (_ct.IsCancellationRequested) return;

      foreach (var itemId in itemsToSpawn)
      {
        if (_ct.IsCancellationRequested) return;

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
