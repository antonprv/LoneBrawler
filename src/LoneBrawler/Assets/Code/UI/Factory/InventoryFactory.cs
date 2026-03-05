// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.UI.Services.DragDropService.Types;
using Code.UI.Services.InventoryService.Interfaces;

using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.UI.Elements.Inventory.Slots;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.UI.Factory
{
  public class InventoryFactory : IInventoryFactory
  {
    private readonly IAssetLoader _assetLoader;
    private readonly IInventoryService _inventoryService;

    public InventoryFactory(
      IAssetLoader assetLoader,
      IInventoryService inventoryService
      )
    {
      _inventoryService = inventoryService;
      _assetLoader = assetLoader;
    }

    #region Public API

    public async UniTask<GameObject> CreateInventorySlotAsync(
      Transform parent,
      int slotIndex,
      DragSource dragSource,
      Canvas parentCanvas
      ) =>
      await InitializeInventorySlotAsync(
        parent,
        slotIndex,
        dragSource,
        parentCanvas
        );

    public async UniTask<List<InventorySlotView>> CreateHotbarElementAsync(
      Transform parent, Canvas parentCanvas) =>
      await InitializeHotbarElementAsync(parent, parentCanvas);

    #endregion

    #region Private API

    private async UniTask<GameObject> InitializeInventorySlotAsync(
      Transform parent,
      int slotIndex,
      DragSource dragSource,
      Canvas parentCanvas
      )
    {
      GameObject slotObject =
        await _assetLoader
        .InstantiateAsync(AssetAddresses.InventorySlotAddress, parent);

      slotObject.SetActive(false);

      InventorySlotView slotView = slotObject.GetComponent<InventorySlotView>();
      await slotView.InitializeAsync(slotIndex, dragSource);

      slotObject.SetActive(true);

      return slotObject;
    }

    private async UniTask<List<InventorySlotView>> InitializeHotbarElementAsync(
      Transform parent, Canvas parentCanvas)
    {
      List<InventorySlotView> hotbarSlotViews = new();

      // Clear existing slots
      foreach (Transform child in parent)
        GameObject.Destroy(child.gameObject);

      // Create slots
      for (int slotIndex = 0; slotIndex < _inventoryService.HotbarSize; slotIndex++)
      {
        var slotObject = await _assetLoader.InstantiateAsync(AssetAddresses.HotbarSlotAddress, parent);

        slotObject.SetActive(false);

        slotObject.GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 130f);
        var slotView = slotObject.GetComponent<InventorySlotView>();

        await slotView.InitializeAsync(slotIndex, DragSource.Hotbar);

        slotObject.SetActive(true);

        hotbarSlotViews.Add(slotView);

        await UniTask.Yield();
      }

      return hotbarSlotViews;
    }

    #endregion
  }
}
