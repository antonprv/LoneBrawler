// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.UI.Factory.Interfaces
{
  public interface IShopItemFactory
  {
    /// <summary>
    /// Creates a shop UI element and returns its GameObject.
    /// </summary>
    /// <param name="itemId">Shop item identifier</param>
    /// <param name="parent">Parent Transform for the created object</param>
    /// <returns>GameObject with ShopItemView component</returns>
    UniTask<GameObject> CreateShopItem(BuffClassName className, Transform parent);
  }
}
