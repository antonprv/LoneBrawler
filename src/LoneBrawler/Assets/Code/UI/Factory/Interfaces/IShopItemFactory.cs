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
    /// Создает UI элемент магазина и возвращает его GameObject.
    /// </summary>
    /// <param name="itemId">Идентификатор элемента магазина</param>
    /// <param name="parent">Родительский Transform для созданного объекта</param>
    /// <returns>GameObject с компонентом ShopItemView</returns>
    UniTask<GameObject> CreateShopItem(BuffClassName className, Transform parent);
  }
}
