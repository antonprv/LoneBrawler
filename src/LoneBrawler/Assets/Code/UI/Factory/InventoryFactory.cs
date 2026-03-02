// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.DragDropService.Interfaces;
using Code.UI.Elements.Inventory;
using Code.UI.Elements.Inventory.Slots;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Code.UI.Factory.Interfaces;
using System.Threading.Tasks;
using System;
using Code.External.Infrastructure.Unity;

namespace Code.UI.Factory
{
  public class InventoryFactory : IInventoryFactory
  {
    private readonly IAssetLoader _assetLoader;

    public InventoryFactory(IAssetLoader assetLoader) => _assetLoader = assetLoader;

    #region Public API

    public async UniTask<GameObject> CreateInventorySlotAsync(
      ItemTooltipController tooltipController,
      int slotIndex,
      DragSource dragSource,
      Canvas parentCanvas,
      RectTransform dragLayer
      ) =>
      await InitializeInventorySlotAsync(
        tooltipController,
        slotIndex,
        dragSource,
        parentCanvas,
        dragLayer
        );

    public async UniTask<GameObject> CreateHotbarElementAsync(Transform parent) =>
      await InitializeHotbarElementAsync(parent);

    #endregion

    #region Private API

    private async UniTask<GameObject> InitializeInventorySlotAsync(
      ItemTooltipController tooltipController,
      int slotIndex,
      DragSource dragSource,
      Canvas parentCanvas,
      RectTransform dragLayer
      )
    {
      GameObject slotObject = await _assetLoader.InstantiateAsync(AssetAddresses.InventorySlotAddress);

      slotObject.SetActive(false);

      InventorySlotView slotView = slotObject.GetComponent<InventorySlotView>();
      slotView.Construct(tooltipController);
      await slotView.InitializeAsync(slotIndex, dragSource, parentCanvas, dragLayer);

      slotObject.SetActive(true);

      return slotObject;
    }

    private async UniTask<GameObject> InitializeHotbarElementAsync(Transform parent)
    {
      var elementObject = await _assetLoader.InstantiateAsync(AssetAddresses.HotbarSlotAddress);

      elementObject.SetActive(false);

      elementObject.GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 130f);



      elementObject.SetActive(true);

      return elementObject;
    }

    #endregion
  }
}
