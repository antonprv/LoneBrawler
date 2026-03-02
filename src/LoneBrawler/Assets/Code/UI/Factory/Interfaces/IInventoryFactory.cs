// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DragDropService.Interfaces;
using Code.UI.Elements.Inventory;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.UI.Factory.Interfaces
{
  public interface IInventoryFactory
  {
    UniTask<GameObject> CreateHotbarElementAsync(Transform parent);
    UniTask<GameObject> CreateInventorySlotAsync(
      ItemTooltipController tooltipController,
      int slotIndex,
      DragSource dragSource,
      Canvas parentCanvas,
      RectTransform dragLayer
      );
  }
}
