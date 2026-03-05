// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.UI.Services.DragDropService.Types;

using Code.UI.Elements.Inventory.Slots;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.UI.Factory.Interfaces
{
  public interface IInventoryFactory
  {
    UniTask<List<InventorySlotView>> CreateHotbarElementAsync(
      Transform parent,
      Canvas parentCanvas
      );

    UniTask<GameObject> CreateInventorySlotAsync(
      Transform parent,
      int slotIndex,
      DragSource dragSource,
      Canvas parentCanvas
      );
  }
}
