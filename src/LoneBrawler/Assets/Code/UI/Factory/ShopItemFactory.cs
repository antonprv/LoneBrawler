// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.UI.Factory.Interfaces;

using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.UI.Elements.Shop;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.UI.Factory
{
  public class ShopItemFactory : IShopItemFactory
  {
    private readonly IAssetLoader _assetLoader;
    private readonly IBuffDataSubservice _shopItemData;
    private readonly IGameLog _logger;

    public ShopItemFactory(
      IAssetLoader assetLoader,
      IBuffDataSubservice buffData,
      IGameLog gameLog
      )
    {
      _assetLoader = assetLoader;
      _shopItemData = buffData;
      _logger = gameLog;
    }

    #region Public API

    /// <summary>
    /// Creates a store UI element and returns its GameObject.
    /// </summary>
    public async UniTask<GameObject> CreateShopItem(BuffClassName buffClassName, Transform parent)
    {
      if (buffClassName == BuffClassName.None || buffClassName == BuffClassName.BuffBase)
      {
        _logger.Log(LogType.Error,
          $"[ShopItemFactory] Attempt to create shop item with type {buffClassName} — this is not allowed.");
        return null;
      }

      return await InstantiateShopItem(buffClassName, parent);
    }

    #endregion

    #region Private API

    private async UniTask<GameObject> InstantiateShopItem(BuffClassName buffClassName, Transform parent)
    {
      try
      {
        BuffStaticData itemData = await _shopItemData.ForBuffAsync(buffClassName);

        if (itemData == null)
        {
          _logger.Log(LogType.Error,
            $"[ShopItemFactory] ShopItemStaticData for '{buffClassName}' was not found in manifest.");
          return null;
        }

        GameObject itemObject = await _assetLoader.InstantiateAsync(
          itemData.ShopItemPrefabReference,
          parent
        );

        if (itemObject == null)
        {
          _logger.Log(LogType.Error,
            $"[ShopItemFactory] Failed to instantiate prefab for '{buffClassName}'.");
          return null;
        }

        ShopItemView itemView = itemObject.GetComponent<ShopItemView>();

        if (itemView == null)
        {
          _logger.Log(LogType.Error,
            $"[ShopItemFactory] ShopItemView component not found on prefab for '{buffClassName}'.");
          GameObject.Destroy(itemObject);
          return null;
        }

        // Loading icon
        if (itemData.Icon != null)
        {
          Sprite iconSprite = await _assetLoader.LoadAsync<Sprite>(itemData.Icon);

          if (iconSprite != null)
            itemView.SetIcon(iconSprite);
        }

        // Initializing with required ID
        itemView.Initialize(buffClassName);

        return itemObject;
      }
      catch (Exception ex)
      {
        _logger.Log(LogType.Error,
          $"[ShopItemFactory] Failed to instantiate shop item '{buffClassName}': {ex.Message}");
        return null;
      }
    }

    #endregion
  }
}
